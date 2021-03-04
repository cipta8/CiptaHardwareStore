using System.Collections.Generic;
using System.Linq;
using CatalogMergeFeature.Entities;
using CatalogMergeFeature.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CatalogMergeFeature.Services
{
    public class MergeCatalogsService : IMergeCatalogsService
    {
        private readonly ILogger<MergeCatalogsService> _logger;

        public MergeCatalogsService(ILogger<MergeCatalogsService> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// This method contains logic to merge several list of stock into single list of stock that only
        /// contains distinct barcode.
        /// </summary>
        /// <param name="inputStocks"></param>
        /// <returns></returns>
        public IEnumerable<Stock> CombineStock(IEnumerable<IEnumerable<Stock>> inputStocks)
        {
            if (!inputStocks.Any() || inputStocks.All(inputCatalog => !inputCatalog.Any()))
            {
                return null;
            }
            
            var mergedStocks = inputStocks.SelectMany(ic => ic);
            _logger.LogDebug($"Step 01 - Merge {mergedStocks.Count()} stocks");
            
            var distinctBarcodes = mergedStocks.GroupBy(mc => mc.Barcode);
            _logger.LogDebug($"Step 02 - Get distinct {distinctBarcodes.Count()} barcodes from merged {mergedStocks.Count()} stocks");

            var distinctNewStocks = distinctBarcodes.Select(db => new Stock()
                {
                    Barcode = db.Key,
                    SKU = db.First().SKU,
                    SupplierName = db.First().SupplierName,
                    Source = db.First().Source,
                    Description = db.First().Description
                })
                .OrderBy(s => s.Barcode)
                .GroupBy(s => s.SKU)
                .Select(gs => gs.First());
            
            _logger.LogDebug($"Step 03 - Generate {distinctNewStocks.Count()} unique stocks");
            
            return distinctNewStocks;
        }
    }
}