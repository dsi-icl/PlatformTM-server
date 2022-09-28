// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Text.Json;
using Loader;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM;

using PlatformTM.Models;

TabularMapper tabularMapper = MapperService.ReadMappingFile("/Users/iemam/Box/UNICORN - Data FAIRification/BT/Mappers/v2/BT_RD_R_mapper.csv");

const string outputDir = "/Users/iemam/Box/UNICORN - Data FAIRification/BT/Descriptors/";
const string mappersDir = "/Users/iemam/Box/UNICORN - Data FAIRification/BT/Mappers/";

Dictionary<string, DatasetMapper> mappers = MapperService.ProcessTabularMapper(tabularMapper);

var descriptors = MapperService.InitObsDescriptors(mappers.Values.ToList());
List<DataTable> descriptors_dtList = MapperService.TabulariseDescriptors(descriptors);

foreach(var descriptorDT in descriptors_dtList)
{
    TabularDataIO.WriteDataFile(Path.Combine(outputDir,descriptorDT.TableName.Replace(' ','_')+".csv"), descriptorDT);
}

//foreach(var descriptor in descriptors)
//{

//    var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10 };
//    string jsonString = JsonSerializer.Serialize(descriptor, options);

   
//    string JsonOutputFile = "./" + descriptor.Title + "_DESCRIPTOR.json"; 
//    File.WriteAllText(JsonOutputFile, jsonString);
//}





//Will then iterate over the DatasetMappers to create a Primary Dataset for each dataset,
//which involve reading in all the source data files that are referenced from each dataset
//create a primary dataset, and adding dataset records using references from the descriptor fields
//which means that the descriptor fields need to be 



