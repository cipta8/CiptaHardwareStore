namespace CatalogMergeFeature.Entities
{
    public class Stock : DisplayStock
    {
        public string Barcode { get; set; }
        public string SupplierName { get; set; }
    }
}