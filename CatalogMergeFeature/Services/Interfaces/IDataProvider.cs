using System.Collections.Generic;

namespace CatalogMergeFeature.Services.Interfaces
{
    public interface IDataProvider
    {
        IList<T> Load<T>(string uri);
        string ToString<T>(IEnumerable<T> inputList);
        void Store<T>(string uri, IEnumerable<T> inputList);
    }
}