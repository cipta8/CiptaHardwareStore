using System;
using System.Collections.Generic;
using System.Linq;
using CatalogMergeFeature;
using CatalogMergeFeature.Entities;
using CatalogMergeFeature.Entities.Setting;
using CatalogMergeFeature.Infrastructure;
using CatalogMergeFeature.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace CatalogMergeFeatureTest
{
    public class FeatureTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public FeatureTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        
        [Fact]
        public void FileHelperTest()
        {
            var files = new string[]
            {
                @"C:\Test\catalogJ.csv",
                @"C:\Test\suppliersK.csv",
                @"C:\Test\barcodesOPQ.csv",
            };

            using IHost host = CreateHostBuilder(new string[] {}).Build();

            var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            
            var setting = provider.GetRequiredService<IOptions<MergeSetting>>().Value;

            var fileSources = files.GetFileSources(setting.FileTypes);

            var supplierFileSource = fileSources.FirstOrDefault(fs =>
                fs.Type.EntityType.Equals(nameof(Supplier), StringComparison.OrdinalIgnoreCase));
            
            Assert.NotNull(supplierFileSource);
            Assert.Equal("K",supplierFileSource.Source);
            Assert.Equal(@"C:\Test\suppliersK.csv",supplierFileSource.FilePath);
            
            var catalogFileSource = fileSources.FirstOrDefault(fs =>
                fs.Type.EntityType.Equals(nameof(Catalog), StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(catalogFileSource);
            Assert.Equal("J",catalogFileSource.Source);
            Assert.Equal(@"C:\Test\catalogJ.csv", catalogFileSource.FilePath);            
            
            var barcodeFileSource = fileSources.FirstOrDefault(fs =>
                fs.Type.EntityType.Equals(nameof(SupplierProductBarcode), StringComparison.OrdinalIgnoreCase));
            Assert.NotNull(barcodeFileSource);
            Assert.Equal("OPQ",barcodeFileSource.Source);
            Assert.Equal(@"C:\Test\barcodesOPQ.csv", barcodeFileSource.FilePath);     
        }
        
        
        [Fact]
        public void MergeSettingExtensionsTest()
        {
            var setting = new MergeSetting()
            {
                DataFileExtension = "csv",
                OutputFileName = "test_output_result.csv",
                ReferenceOutputFileName = "reference_result_Output.csv",
                Folder = new FolderSetting()
                {
                    Base = @"C:\UnitTest",
                    Input = "incomingData",
                    Output = "exportedData",
                    Reference = "referenceData"
                }
            };
            var inputFolder = setting.InputFolder();
            var outputFile = setting.OutputFilePath();
            var varsionedOutputFile = setting.OutputFilePath("20210304_171500");
            var referenceFile = setting.ReferenceOutputFilePath();
            
            Assert.Equal(@"C:\UnitTest\incomingData\", inputFolder);
            Assert.Equal(@"C:\UnitTest\exportedData\test_output_result.csv", outputFile);
            Assert.Equal(@"C:\UnitTest\exportedData\test_output_result_20210304_171500.csv", varsionedOutputFile);
            Assert.Equal(@"C:\UnitTest\referenceData\reference_result_Output.csv", referenceFile);
        }

        [Fact]
        public void CompileStockTest()
        {
            var args = new string[] { };
            
            using IHost host = CreateHostBuilder(args).Build();

            var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            
            var stockProvider = provider.GetRequiredService<IStockProvider>();

            var testSource = "XYZ";
            var suppliers = new List<Supplier>()
            {
                new Supplier() {ID = 567, Name = "ABCD"},
                new Supplier() {ID = 789, Name = "EFG"},
            };
            var catalogs = new List<Catalog>()
            {
                new Catalog() {SKU = "1234", Description = "Item 1234"},
                new Catalog() {SKU = "5678", Description = "Item 5678"},
            };

            var barcodes = new List<SupplierProductBarcode>()
            {
                new SupplierProductBarcode() {Barcode = "ABCD1234", SKU = "1234", SupplierID = 567},
                new SupplierProductBarcode() {Barcode = "JKLM5678", SKU = "5678", SupplierID = 789}
            };
 
            var compiledStocks = stockProvider.CompileStocks(
                testSource, 
                suppliers,
                catalogs,
                barcodes);
            
            Assert.Equal("ABCD",compiledStocks[0].SupplierName);
            Assert.Equal("EFG",compiledStocks[1].SupplierName);
            Assert.Equal("XYZ",compiledStocks[0].Source);
            Assert.Equal("XYZ",compiledStocks[1].Source);
            Assert.Equal("1234",compiledStocks[0].SKU);
            Assert.Equal("5678",compiledStocks[1].SKU);
            Assert.Equal("ABCD1234",compiledStocks[0].Barcode);
            Assert.Equal("JKLM5678",compiledStocks[1].Barcode);
        }

        [Fact]
        public void CombineStockTest()
        {
            var args = new string[] { };
            
            using IHost host = CreateHostBuilder(args).Build();

            var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            
            var catalogMerger = provider.GetRequiredService<IMergeCatalogsService>();

            var listStockABC = new List<Stock>()
            {
                new Stock() { Barcode = "ABCD1234", Description = "ABCD1234 Description BBE", SKU = "BBE", SupplierName = "SupplierX", Source = "ABC"},
                new Stock() { Barcode = "EFG56789", Description = "EFG56789 Description 27X", SKU = "27X", SupplierName = "SupplierD", Source = "ABC"},
                new Stock() { Barcode = "HIJ87654", Description = "HIJ87654 Description QAC", SKU = "QAC", SupplierName = "SupplierX", Source = "ABC"},
                new Stock() { Barcode = "LSJ22831", Description = "LSJ22831 Description 90A", SKU = "90A", SupplierName = "SupplierZ", Source = "ABC"},
                new Stock() { Barcode = "ZZS26619", Description = "ZZS26619 Description 56S", SKU = "56S", SupplierName = "SupplierD", Source = "ABC"},
            };
            
            var listStockDEF = new List<Stock>()
            {
                new Stock() { Barcode = "MKN88293", Description = "MKN88293 Description 29AD", SKU = "29AD", SupplierName = "SupplierX", Source = "DEF"},
                new Stock() { Barcode = "HIJ87654", Description = "HIJ87654 Description 90A", SKU = "90A", SupplierName = "SupplierA", Source = "DEF"},
                new Stock() { Barcode = "ABCD1234", Description = "ABCD1234 Description J29", SKU = "J29", SupplierName = "SupplierJ", Source = "DEF"},
                new Stock() { Barcode = "SCK9922D", Description = "SCK9922D Description CHS", SKU = "CHS", SupplierName = "SupplierD", Source = "DEF"},
            };
            
            var listStockXYZ = new List<Stock>()
            {
                new Stock() { Barcode = "CSA002AS", Description = "CSA002AS Description OPSA", SKU = "OPSA", SupplierName = "SupplierA", Source = "XYZ"},
                new Stock() { Barcode = "ABCD1234", Description = "ABCD1234 Description", SKU = "FSG", SupplierName = "SupplierA", Source = "XYZ"},
                new Stock() { Barcode = "LSJ22831", Description = "LSJ22831 Description QWE", SKU = "QWE", SupplierName = "SupplierG", Source = "XYZ"},
                new Stock() { Barcode = "MKN88293", Description = "MKN88293 Description BBSA", SKU = "BBSA", SupplierName = "SupplierX", Source = "XYZ"},
            };

            var stocks = new List<List<Stock>>()
            {
                listStockABC,
                listStockDEF,
                listStockXYZ,
            };
 
            var mergedStocks = catalogMerger.CombineStock(stocks).ToList();
            
            Assert.NotNull(mergedStocks);
            Assert.True(mergedStocks.Any());
            Assert.Equal(8, mergedStocks.Count());
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("ABCD1234", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("EFG56789", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("HIJ87654", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("LSJ22831", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("ZZS26619", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("MKN88293", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("SCK9922D", StringComparison.OrdinalIgnoreCase)));
            Assert.NotNull(mergedStocks.FirstOrDefault(ms => ms.Barcode.Equals("CSA002AS", StringComparison.OrdinalIgnoreCase)));
            
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("ABCD1234", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("EFG56789", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("HIJ87654", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("LSJ22831", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("ZZS26619", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("MKN88293", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("SCK9922D", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(1,mergedStocks.Count(ms => ms.Barcode.Equals("CSA002AS", StringComparison.OrdinalIgnoreCase)));

            
            
            Assert.Equal("ABC",mergedStocks.First(ms => ms.Barcode.Equals("ABCD1234", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("ABC",mergedStocks.First(ms => ms.Barcode.Equals("EFG56789", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("ABC",mergedStocks.First(ms => ms.Barcode.Equals("HIJ87654", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("ABC",mergedStocks.First(ms => ms.Barcode.Equals("LSJ22831", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("ABC",mergedStocks.First(ms => ms.Barcode.Equals("ZZS26619", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("DEF",mergedStocks.First(ms => ms.Barcode.Equals("MKN88293", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("DEF",mergedStocks.First(ms => ms.Barcode.Equals("SCK9922D", StringComparison.OrdinalIgnoreCase)).Source);
            Assert.Equal("XYZ",mergedStocks.First(ms => ms.Barcode.Equals("CSA002AS", StringComparison.OrdinalIgnoreCase)).Source);
        }        

        [Fact]
        public void MergeTest()
        {
            var args = new string[] { };
            
            using IHost host = CreateHostBuilder(args).Build();

            var serviceScope = host.Services.CreateScope();
            var provider = serviceScope.ServiceProvider;
            
            var setting = provider.GetRequiredService<IOptions<MergeSetting>>().Value;
            
            var referenceDataLocation = setting.ReferenceOutputFilePath();
            
            var testDataProvider = provider.GetRequiredService<IDataProvider>();
            var referenceData = testDataProvider
                .Load<DisplayStock>(referenceDataLocation)
                .OrderBy(d=>d.SKU);

            try
            {
                CatalogMergeFeature.Main.Execute(provider);
            }
            catch (Exception ex)
            {
                _testOutputHelper.WriteLine($"Error: {ex.Message}\r\nStackTrace:{ex.StackTrace}");
                Environment.Exit(0);
            }
            
            var outputPath = setting.OutputFilePath();
            var outputData = testDataProvider
                .Load<DisplayStock>(outputPath)
                .OrderBy(d => d.SKU);
            
            Assert.True(referenceData
                .All(rd => outputData
                    .FirstOrDefault(od => od.GetSignature()
                        .Equals(rd.GetSignature(), StringComparison.OrdinalIgnoreCase)) != null
                )
            );
        }
        
        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            hostBuilder.ConfigureServices((hostContext, services) =>
                services.AddCatalogMergeFeature(hostContext.Configuration)
            );

            return hostBuilder;
        }
    }
}