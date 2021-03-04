using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CatalogMergeFeature.Entities;
using CatalogMergeFeature.Entities.Setting;

namespace CatalogMergeFeature.Infrastructure
{
    public static class FileHelper
    {
        /// <summary>
        /// This method is used to generate file tag for csv file, any entity type that are not defined in dataFileTypes
        /// will be neglected.
        /// </summary>
        /// <param name="files">List of csv files retrieved from a folder</param>
        /// <param name="dataFileTypes">Entity data type</param>
        /// <returns></returns>
        public static IList<FileSource> GetFileSources(this string[] files, IEnumerable<DataFileType> dataFileTypes)
        {
            if (files.Length == 0)
            {
                return new List<FileSource>();
            }

            var fileSources = files.Select(file =>
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var dataFileType = dataFileTypes
                        .FirstOrDefault(dft => fileName.StartsWith(dft.Prefix, StringComparison.OrdinalIgnoreCase));

                    if (dataFileType == null)
                    {
                        return null;
                    }

                    return new FileSource()
                    {
                        Source = fileName.Replace(dataFileType.Prefix, "", StringComparison.OrdinalIgnoreCase),
                        Type = dataFileType,
                        FilePath = file
                    };
                })
                .Where(fs => fs != null)
                .ToList();

            return fileSources;
        }
    }
}