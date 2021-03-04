# Merge Catalog Console Application

This application is written using C# using .NET 5.0 console application template; shows a possible solution to https://github.com/tosumitagrawal/codingskills.

JetBrains Rider 2020.3 is being used to create this application.

### Project Modules
The solution consist of 3 projects:
```
PROJECT NAME          | DESCRIPTION
--------------------------------------------------------------------------------------------------------------
CatalogMergeFeature   | Main classes of CatalogMerge
CatalogMergeTest      | Unit Test for CatalogMerge using xUnit, has appsettings.json for testing purpose
CatalogMergeHost      | Console application that host CatalogMergeFeature for execution, has appsettings.json
```

### Application Configuration
appsettings.json is the configuration file of the application. Default values are as shown below:
```
"MergeSetting" : {
	"OutputFileName" : "result_output_cipta.csv",
	"ReferenceOutputFileName" : "result_output.csv",
	"DataFileExtension": "csv",
	"Folder" : {
		"Base": "TestData",
		"Input": "input",
		"Output": "output",
		"Reference": "reference"
	},
	"FileTypes" : [
		{
			"EntityType" : "Catalog",
			"Prefix": "catalog"
		},
		{
			"EntityType" : "Supplier",
			"Prefix": "suppliers"
		},
		{
			"EntityType" : "SupplierProductBarcode",
			"Prefix": "barcodes"
		}
	]
}
```

### Important configuration settings:
```
COMMAND LINE ARGUMENT         | DESCRIPTION
-----------------------------------------------------------------------------------------------------------------------------
MergeSetting                  | The setting class of this application, root node of configuration
MergeSetting:OutputFileName   | Specify the output file name of this application; data will always written in CSV format
MergeSetting:Folder:Base      | Base working folder where data is available
MergeSetting:Folder:Input     | Input csv data files, relative to Folder:Base
MergeSetting:Folder:Output    | Output csv data file (merged catalog) is written to folder, relative to Folder:Base
MergeSetting:Folder:Reference | Reference csv data file (merged catalog) that becomes test reference, relative to Folder:Base
```

### How to install
Copy all files and folder inside *Distribution* folder into desired deployment folder.

### How to run
From deployment folder, simply run *CatalogMerge.exe* and it will use default configuration as shown in ```Application Configuration``` section

CatalogMerge.exe accept command line argument as input parameters; it will override configuration values defined in appsettings.json file.
To override a setting, simply add 
```--<COMMAND LINE ARGUMENT>``` after type *CatalogMerge* example:
```
CatalogMerge --MergeSetting:Folder:Base E:\ --MergeSetting:OutputFileName cipta_output.csv*
```

#### Created by Cipta Budhysutanto - March 2021