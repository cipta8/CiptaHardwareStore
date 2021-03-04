using System.IO;
using CatalogMergeFeature.Entities.Setting;

namespace CatalogMergeFeature.Infrastructure
{
    public static class MergeSettingExtensions
    {
        /// <summary>
        /// Get InputFolder from MergeSetting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string InputFolder(this MergeSetting setting)
        {
            return $"{setting.Folder.Base}{Path.DirectorySeparatorChar}" +
                   $"{setting.Folder.Input}{Path.DirectorySeparatorChar}";
        }
        
        /// <summary>
        /// Get Output file path from MergeSetting
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="versionNo"></param>
        /// <returns></returns>
        public static string OutputFilePath(this MergeSetting setting, string versionNo = null)
        {
            if (string.IsNullOrWhiteSpace(versionNo))
            {
                return $"{setting.Folder.Base}{Path.DirectorySeparatorChar}" +
                       $"{setting.Folder.Output}{Path.DirectorySeparatorChar}" +
                       $"{setting.OutputFileName}";
            }

            var fileName = Path.GetFileNameWithoutExtension(setting.OutputFileName);
            var extension = Path.GetExtension(setting.OutputFileName);
            var versionedFileName = $"{fileName}_{versionNo}{extension}";
            return $"{setting.Folder.Base}{Path.DirectorySeparatorChar}" +
                   $"{setting.Folder.Output}{Path.DirectorySeparatorChar}" +
                   $"{versionedFileName}";
        }
        
        /// <summary>
        /// Get Reference Output file path from MergeSetting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static string ReferenceOutputFilePath(this MergeSetting setting)
        {
            return $"{setting.Folder.Base}{Path.DirectorySeparatorChar}" +
                   $"{setting.Folder.Reference}{Path.DirectorySeparatorChar}" +
                   $"{setting.ReferenceOutputFileName}";
        }
    }
}