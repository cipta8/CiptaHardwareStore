using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatalogMergeFeature.Entities;
using CatalogMergeFeature.Entities.Setting;
using CatalogMergeFeature.Services.Interfaces;
using CatalogMergeFeature.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CatalogMergeFeature.Services
{
    public class StockProvider : IStockProvider
    {
        private readonly ILogger<StockProvider> _logger;
        public IDataProvider Provider { get; set; }

        private readonly MergeSetting _setting;

        public StockProvider(
            IDataProvider dataProvider,
            IOptions<MergeSetting> setting,
            ILogger<StockProvider> logger
            )
        {
            _logger = logger;
            this.Provider = dataProvider;
            _setting = setting.Value;
        }

        /// <summary>
        /// This method will retrieve all CSV filenames from specified input folder and transform it into a DTO
        /// called FileSource which contain information about Source of data, csv file name, and entity type of
        /// the CSV file whether it is Supplier, Catalog, or SupplierProductBarcode 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IEnumerable<Stock>> LoadDataFiles()
        {
            //Get all files in input folder
            var files = Directory.GetFiles(_setting.InputFolder(),
                $"*.{_setting.DataFileExtension}");

            _logger.LogInformation($"Success: {files.Length} Data files loaded");
            
            var fileSource = files.GetFileSources(_setting.FileTypes);

            // Only process fileSources grouped by source that has catalog, supplier, and barcode files set.
            var fileSourceGroups = fileSource
                .GroupBy(fs => fs.Source)
                .Where(fs =>
                    fs.Count() == 3
                    && fs.Any(fsm => fsm.Type.EntityType == nameof(Catalog))
                    && fs.Any(fsm => fsm.Type.EntityType == nameof(Supplier))
                    && fs.Any(fsm => fsm.Type.EntityType == nameof(SupplierProductBarcode)));

            var stocksFromSources = fileSourceGroups.Select(GenerateStock).ToArray();

            _logger.LogInformation($"{stocksFromSources.Length} sets of source catalogs loaded.");
            
            return stocksFromSources;
        }

        /// <summary>
        /// This method is to 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private IList<Stock> GenerateStock(IGrouping<string, FileSource> group)
        {
            var catalogUri = group.First(fsm => fsm.Type.EntityType == nameof(Catalog)).FilePath;
            var supplierUri = group.First(fsm => fsm.Type.EntityType == nameof(Supplier)).FilePath;
            var barcodeUri = group.First(fsm => fsm.Type.EntityType == nameof(SupplierProductBarcode)).FilePath;
                    
            var suppliers = DataLoad<Supplier>(supplierUri);
            var catalogs = DataLoad<Catalog>(catalogUri);
            var barcodes = DataLoad<SupplierProductBarcode>(barcodeUri);

            return CompileStocks(group.Key, suppliers, catalogs, barcodes);
        }

        /// <summary>
        /// Loading CSV data into a specific entity
        /// </summary>
        /// <param name="uri"></param>
        /// <typeparam name="T">Entity type that will be loaded from CSV file (Supplier, Catalog, SupplierProductBarcode)</typeparam>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        private IList<T> DataLoad<T>(string uri)
        {
            var entities = this.Provider.Load<T>(uri);
            
            if (entities == null || !entities.Any())
            {
                throw new ApplicationException($"Cannot find {nameof(T)} data.");
            }

            return entities;
        }
        
        /// <summary>
        /// This method output Stock entities which is denormalized version of Supplier, Catalog, and SupplierProductBarcode
        /// </summary>
        /// <param name="source"></param>
        /// <param name="suppliers"></param>
        /// <param name="catalogs"></param>
        /// <param name="barcodes"></param>
        /// <returns></returns>
        public IList<Stock> CompileStocks(
            string source,
            IList<Supplier> suppliers,
            IList<Catalog> catalogs,
            IList<SupplierProductBarcode> barcodes)
        {
            var stocks = barcodes
                .Join(suppliers,
                    barcode => barcode.SupplierID,
                    supplier => supplier.ID,
                    (barcode, supplier) => new Stock()
                    {
                        Barcode = barcode.Barcode,
                        SupplierName = supplier.Name,
                        SKU = barcode.SKU,
                        Source = source,
                        Description = ""
                    })
                .Join(catalogs,
                    stock => stock.SKU,
                    catalog => catalog.SKU,
                    (stock, catalog) =>
                    {
                        stock.Description = catalog.Description;
                        return stock;
                    })
                .ToList();

            return stocks;
        }
    }
}