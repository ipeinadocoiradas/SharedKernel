using BankAccounts.Infrastructure.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace BankAccounts.Api
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args"></param>
        [ExcludeFromCodeCoverage]
        public static async Task<int> Main(string[] args)
        {
            try
            {
                Log.Information("Starting web host");
                var ct = CancellationToken.None;
                var host = await CreateHostBuilder(args).Build().MigrateAsync(ct);
                await host.RunAsync(ct);
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }

        }

        /// <summary>
        /// CreateHostBuilder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}