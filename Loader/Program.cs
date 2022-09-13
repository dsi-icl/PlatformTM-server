// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using PlatformTM;
using PlatformTM.MapperModels;
using PlatformTM.Models;

List<TabularObsMapper> MapperRecords = MapperService.ReadMappingFile("/Users/iemam/Box/UNICORN - Data FAIRification/BT/Mappers/v2/BT_RD_R_mapper.csv");



Dictionary<string, DatasetMapper> mappsers = MapperService.ProcessTabularMapper(MapperRecords);

var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10 };
string jsonString = JsonSerializer.Serialize(mappsers, options);

const string JsonOutputFile = "./mapper2.json";
File.WriteAllText(JsonOutputFile, jsonString);

