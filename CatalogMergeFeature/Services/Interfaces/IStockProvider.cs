using System.Collections.Generic;
using CatalogMergeFeature.Entities;

namespace CatalogMergeFeature.Services.Interfaces
{
    public interface IStockProvider
    {
        public IDataProvider Provider { get; set; }

        public IEnumerable<IEnumerable<Stock>> LoadDataFiles();

        IList<Stock> CompileStocks(
            string source,
            IList<Supplier> suppliers,
            IList<Catalog> catalogs,
            IList<SupplierProductBarcode> barcodes);
    }
}