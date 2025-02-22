﻿using BankAccounts.Application.Shared;
using BankAccounts.Application.Shared.UnitOfWork;
using BankAccounts.Domain.BankAccounts.Events;
using BankAccounts.Domain.BankAccounts.Repository;
using BankAccounts.Domain.Services;
using BankAccounts.Infrastructure.BankAccounts;
using BankAccounts.Infrastructure.BankAccounts.Commands.Validators;
using BankAccounts.Infrastructure.Shared.Data;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Communication.Email.Smtp;
using SharedKernel.Infrastructure.Cqrs.Commands;
using SharedKernel.Infrastructure.Cqrs.Middlewares;
using SharedKernel.Infrastructure.Cqrs.Queries;
using SharedKernel.Infrastructure.Data.Dapper;
using SharedKernel.Infrastructure.Data.EntityFrameworkCore;
using SharedKernel.Infrastructure.Events;
using SharedKernel.Infrastructure.System;
using SharedKernel.Infrastructure.Validators;

namespace BankAccounts.Infrastructure.Shared
{
    /// <summary>
    /// Configurar la inyección de dependencias
    /// </summary>
    public static class BankAccountServiceCollection
    {
        public static IServiceCollection AddBankAccounts(this IServiceCollection serviceCollection,
            IConfiguration configuration, string connectionStringName)
        {
            return serviceCollection
                .AddSharedKernel()
                .AddDomain()
                .AddApplication()
                .AddInfrastructure(configuration, connectionStringName);
        }

        private static IServiceCollection AddDomain(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<BankTransferService>();

            return serviceCollection;
        }

        private static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddDomainEvents(typeof(BankAccountCreated))
                .AddDomainEventsSubscribers(typeof(IBankAccountUnitOfWork).Assembly)
                .AddCommandsHandlers(typeof(IBankAccountUnitOfWork))
                .AddQueriesHandlers(typeof(BankAccountDbContext))
                .AddValidators(typeof(CreateBankAccountValidator));
        }

        private static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection,
            IConfiguration configuration, string connectionStringName)
        {
            serviceCollection
                .AddSmtp(configuration);


            serviceCollection
                .AddFromMatchingInterface(ServiceLifetime.Transient, typeof(IBankAccountRepository),
                    typeof(EntityFrameworkBankAccountRepository), typeof(IBankAccountUnitOfWork));

            // Repositories
            serviceCollection.AddTransient<IBankAccountRepository, EntityFrameworkBankAccountRepository>();

            // Unit of work
            serviceCollection.AddScoped<IBankAccountUnitOfWork, BankAccountDbContext>();
            serviceCollection.AddEntityFrameworkCoreSqlServer<BankAccountDbContext>(configuration, connectionStringName);

            // Dapper
            serviceCollection.AddDapperSqlServer(configuration, connectionStringName);

            serviceCollection
                .AddValidationMiddleware()
                .AddRetryPolicyMiddleware<BankAccountRetryPolicyExceptionHandler>()
                .AddTimerMiddleware();

            return serviceCollection;
        }
    }
}
