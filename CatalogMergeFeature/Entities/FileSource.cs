using CatalogMergeFeature.Entities.Setting;

namespace CatalogMergeFeature.Entities
{
    public class FileSource
    {
        public string Source { get; set; }
        public DataFileType Type { get; set; } 
        public string FilePath { get; set; }
    }
}