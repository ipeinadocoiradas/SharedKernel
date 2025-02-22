﻿using SharedKernel.Application.Cqrs.Queries;
using SharedKernel.Application.Cqrs.Queries.Contracts;
using SharedKernel.Application.Cqrs.Queries.Entities;
using SharedKernel.Application.Cqrs.Queries.Kendo;
using SharedKernel.Application.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Api.Grids.KendoGrid
{
    /// <summary>
    /// Command bus extensions
    /// </summary>
    public static class QueryBusExtensions
    {
        //private const string DeletedKey = "Deleted";
        //private const string OnlyDeletedKey = "OnlyDeleted";

        /// <summary>
        /// Create query and send it over the command and query bus
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="queryBus"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        /// <returns></returns>
        public static async Task<KendoGridResponse<TResponse>> Ask<T, TResponse>(this IQueryBus queryBus,
            KendoGridRequest<T> request, CancellationToken cancellationToken) where T : IQueryRequest<IPagedList<TResponse>>
        {
            //var showDeleted = ReflectionHelper.GetProperty<T, bool>(request.Filter, DeletedKey);

            //var showOnlyDeleted = ReflectionHelper.GetProperty<T, bool>(request.Filter, OnlyDeletedKey);

            ReflectionHelper.SetProperty(request.Filter, nameof(PageOptions), request.State.ToPageOptions());

            var pagedList = await queryBus.Ask(request.Filter, cancellationToken);

            return new KendoGridResponse<TResponse>(pagedList.Items, pagedList.TotalRecordsFiltered);
        }


    }
}
