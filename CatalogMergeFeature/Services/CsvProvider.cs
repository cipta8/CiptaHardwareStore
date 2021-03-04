using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CatalogMergeFeature.Services.Interfaces;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace CatalogMergeFeature.Services
{
    public class CsvProvider : IDataProvider
    {
        private readonly ILogger<CsvProvider> _logger;

        public CsvProvider(ILogger<CsvProvider> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Loading CSV file and convert it into List of entity
        /// </summary>
        /// <param name="uri"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IList<T> Load<T>(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri) && !File.Exists(uri))
            {
                return null;
            }

            using (var reader = new StreamReader(uri))
            {
                using (var csvFile = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var content = csvFile.GetRecords<T>().ToList();
                    
                    _logger.LogInformation($"Success load CSV file: {uri}");

                    return content;
                }
            }
        }

        /// <summary>
        /// Write list of entity into CSV format and return it as list of string
        /// </summary>
        /// <param name="inputList"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public string ToString<T>(IEnumerable<T> inputList)
        {
            using (var writer = new StringWriter())
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(inputList);
                    csv.Flush();
                }
                return writer.ToString();
            }
        }

        /// <summary>
        /// Method to write list of entity into CSV file
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="inputList"></param>
        /// <typeparam name="T"></typeparam>
        public void Store<T>(string uri, IEnumerable<T> inputList)
        {
            var folder = Path.GetDirectoryName(uri);

            if (string.IsNullOrWhiteSpace(folder))
            {
                _logger.LogError("Unable to save to CSV file.");
            }
            
            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            
            using (var writer = new StreamWriter(uri))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(inputList);
                }
            }
            _logger.LogInformation($"Success write to CSV file: {uri}");
        }
    }
}