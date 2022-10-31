// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;
using Loader;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Models;
using System.Configuration;
using PlatformTM.Core.Domain.Model.DatasetModel;

const string outputDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Output/SMSsurvey/";
const string srcDataDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Data/";
const string mappersDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Mappers/";
const string cdiscTemplatesDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/";
const string cdiscDescriptorDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/json_descriptors/";


//CreatePrimaryDatasets();
CreateDatasetDescriptors();


void CreatePrimaryDatasets()
{
    var appSettings = ConfigurationManager.AppSettings;

    TabularMapper tabularMapper = MapperService.ReadMappingFile("/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Mappers/checked/BT_SurveySMS_mapper.csv");

    List<DatasetMapper> mappers = MapperService.ProcessTabularMapper(tabularMapper);

    foreach (var dsMapper in mappers)
    {
        var PrimaryDS = MapperService.CreateObsDatasets(dsMapper, srcDataDir);
        WriteDSToJSON(PrimaryDS);
       
    }
}

void WriteDSToJSON(PrimaryDataset? PrimaryDS)
{
    var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    string jsonString = JsonSerializer.Serialize(PrimaryDS, options);


    string JsonOutputFile = outputDir + PrimaryDS?.DatasetDescriptor.Title + "_DATA.json";
    File.WriteAllText(JsonOutputFile, jsonString);
}

void CreateDatasetDescriptors()
{

    var mapperFileNames = ConfigurationManager.AppSettings.Get("MapperFileNames").Split(',');
    var descriptorsfullpath = ConfigurationManager.AppSettings.Get("DescriptorsDirectory");

    foreach (var mapper in mapperFileNames)
    {
        var mapperfullpath = Path.Combine(ConfigurationManager.AppSettings.Get("MappersDirectory"), mapper);
        

        TabularMapper tabularMapper = MapperService.ReadMappingFile(mapperfullpath);

        List<DatasetMapper> mappers = MapperService.ProcessTabularMapper(tabularMapper);

        var descriptors = MapperService.InitObsDescriptors(mappers);

        List<DataTable> descriptors_dtList = MapperService.TabulariseDescriptors(descriptors);

        foreach (var descriptorDT in descriptors_dtList)
        {
            TabularDataIO.WriteDataFile(Path.Combine(descriptorsfullpath, descriptorDT.TableName.Replace(' ', '_') + '_'+mapper.Replace("mapper.csv","descriptor")+ ".csv"), descriptorDT);
        }
    }

    

}




void CreateCDISCdescritptors()
{
    //DatasetTemplate template = MapperService.CreateCDISCtemplate(cdiscTemplatesDir, "AG-template-DS.csv", "AG-template-Vars.csv");

    //var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10, DefaultIgnoreCondition= JsonIgnoreCondition.WhenWritingNull };
    //    string jsonString = JsonSerializer.Serialize(template, options);


    //    string JsonOutputFile = cdiscDescriptorDir + template.Id + "_DESCRIPTOR.json"; 
    //    File.WriteAllText(JsonOutputFile, jsonString);

    //###################
}



