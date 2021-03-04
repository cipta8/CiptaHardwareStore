using System;
using System.Linq;
using CatalogMergeFeature.Entities;
using CatalogMergeFeature.Entities.Setting;
using CatalogMergeFeature.Infrastructure;
using CatalogMergeFeature.Services;
using CatalogMergeFeature.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CatalogMergeFeature
{
    public static class Main
    {
        /// <summary>
        /// AddCatalogMergeFeature is used for registering services in Dependency Injection container 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCatalogMergeFeature(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services
                .AddScoped<IDataProvider, CsvProvider>()
                .AddScoped<IStockProvider, StockProvider>()
                .AddScoped<IMergeCatalogsService, MergeCatalogsService>()
                .Configure<MergeSetting>(configuration.GetSection(nameof(MergeSetting)));
        }

        
        /// <summary>
        /// Main entry for execution of CatalogMerge
        /// </summary>
        /// <param name="provider"></param>
        public static void Execute(IServiceProvider provider)
        {
            var setting = provider.GetRequiredService<IOptions<MergeSetting>>().Value;
            
            var stockProvider = provider.GetRequiredService<IStockProvider>();
            var mergeService = provider.GetRequiredService<IMergeCatalogsService>();
           
            var rawStocks = stockProvider.LoadDataFiles();
            var combinedStock = mergeService.CombineStock(rawStocks);

            var displayCombinedStock = combinedStock
                .Select(stock => (DisplayStock) stock)
                .OrderBy(s => s.SKU)
                .ToList();

            var result = stockProvider.Provider.ToString<DisplayStock>(displayCombinedStock);
            
            Console.WriteLine(result);

            var outputPath = setting.OutputFilePath();
            
            stockProvider.Provider.Store<DisplayStock>(outputPath, displayCombinedStock);
        }
    }
}