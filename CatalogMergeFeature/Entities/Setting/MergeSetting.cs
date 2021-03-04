using System.Collections.Generic;

namespace CatalogMergeFeature.Entities.Setting
{
    /// <summary>
    /// This is setting class of CatalogMergeFeature; instance of this class is taken from appsettings.json
    /// </summary>
    public class MergeSetting
    {
        public string OutputFileName { get; set; }
        public string ReferenceOutputFileName { get; set; }
        public string DataFileExtension { get; set; } = "csv";
        public FolderSetting Folder { get; set; }
        public List<DataFileType> FileTypes { get; set; }
    }
}