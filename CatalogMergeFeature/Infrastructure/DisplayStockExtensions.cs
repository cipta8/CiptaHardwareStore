using CatalogMergeFeature.Entities;

namespace CatalogMergeFeature.Infrastructure
{
    public static class DisplayStockExtensions
    {
        /// <summary>
        /// This method generates signature of a DisplayStock entity
        /// </summary>
        /// <param name="stock"></param>
        /// <returns></returns>
        public static string GetSignature(this DisplayStock stock)
        {
            return $"{stock.SKU}-{stock.Description}-{stock.Source}";
        }
    }
}