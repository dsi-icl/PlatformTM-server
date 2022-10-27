// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Loader;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Models;

TabularMapper tabularMapper = MapperService.ReadMappingFile("/Users/iemam/Box/UNICORN - Data FAIRification/BT/Mappers/v2/BT_RD_R_mapper.csv");

const string outputDir = "/Users/iemam/Box/UNICORN - Data FAIRification/BT/Output/";
const string srcDataDir = "/Users/iemam/Box/UNICORN - Data FAIRification/BT/Data/";
const string mappersDir = "/Users/iemam/Box/UNICORN - Data FAIRification/BT/Mappers/";
const string cdiscTemplatesDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/";
const string cdiscDescriptorDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/json_descriptors/";

//DatasetTemplate template = MapperService.CreateCDISCtemplate(cdiscTemplatesDir, "AG-template-DS.csv", "AG-template-Vars.csv");

//var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10, DefaultIgnoreCondition= JsonIgnoreCondition.WhenWritingNull };
//    string jsonString = JsonSerializer.Serialize(template, options);


//    string JsonOutputFile = cdiscDescriptorDir + template.Id + "_DESCRIPTOR.json"; 
//    File.WriteAllText(JsonOutputFile, jsonString);

//###################
List<DatasetMapper> mappers = MapperService.ProcessTabularMapper(tabularMapper);

var PrimaryDS = MapperService.CreateObsDatasets(mappers.First(), srcDataDir);

//var descriptors = MapperService.InitObsDescriptors(mappers);
//List<DataTable> descriptors_dtList = MapperService.TabulariseDescriptors(descriptors);

//foreach (var descriptorDT in descriptors_dtList)
//{
//TabularDataIO.WriteDataFile(Path.Combine(outputDir, descriptorDT.TableName.Replace(' ', '_') + ".csv"), descriptorDT);
//}


//##################
//foreach(var descriptor in descriptors)
//{

var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10 };
string jsonString = JsonSerializer.Serialize(PrimaryDS, options);


    string JsonOutputFile = outputDir + PrimaryDS.DatasetDescriptor.Title + "_DATA.json"; 
   File.WriteAllText(JsonOutputFile, jsonString);
//}





//Will then iterate over the DatasetMappers to create a Primary Dataset for each dataset,
//which involve reading in all the source data files that are referenced from each dataset
//create a primary dataset, and adding dataset records using references from the descriptor fields
//which means that the descriptor fields need to be 



