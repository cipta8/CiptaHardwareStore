using System;
using System.Threading.Tasks;
using CatalogMergeFeature;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CatalogMergeHost
{
    public class Program
    {
        /// <summary>
        /// Main entry for host execution
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            try
            {
                CatalogMergeFeature.Main.Execute(provider);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\r\nStackTrace:{ex.StackTrace}");
            }
            finally
            {
                Environment.Exit(0);
            }
            return host.RunAsync();
        }

        /// <summary>
        /// Create application host using default builder and register services of CatalogMergeFeature to DI container.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureServices((hostContext, services) =>
                services.AddCatalogMergeFeature(hostContext.Configuration)
            );

            return hostBuilder;
        }
    }
}
