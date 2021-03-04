using System.Collections.Generic;
using CatalogMergeFeature.Entities;

namespace CatalogMergeFeature.Services.Interfaces
{
    public interface IMergeCatalogsService
    {
        IEnumerable<Stock> CombineStock(IEnumerable<IEnumerable<Stock>> inputStocks);
    }
}