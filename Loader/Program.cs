// See https://aka.ms/new-console-template for more information

using System.Data;
using System.Text.Json;
using Loader;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM;
using PlatformTM.Models;
using System.Configuration;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using System.Text.Json.Serialization;

//const string outputDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Output/SMSsurvey/";
//const string srcDataDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Data/";
////const string mappersDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Mappers/";
//const string cdiscTemplatesDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/";
//const string cdiscDescriptorDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/json_descriptors/";


//CreatePrimaryDatasets();
CreateDatasetDescriptors();


void CreatePrimaryDatasets()
{
    var mapperFileNames = ConfigurationManager.AppSettings.Get("MapperFileNames").Split(',');
    var sourcDataPath = ConfigurationManager.AppSettings.Get("SourceDataDirectory");
    var outputDataPath = ConfigurationManager.AppSettings.Get("OutputDirectory");
    List<PrimaryDataset> datasets = new();

    foreach (var mapperFileName in mapperFileNames)
    {
        var mapperfullpath = Path.Combine(ConfigurationManager.AppSettings.Get("MappersDirectory"), mapperFileName);

        TabularMapper tabularMapper = MapperService.ReadMappingFile(mapperfullpath);

        List<DatasetMapper> mappers = MapperService.ProcessTabularMapper(tabularMapper);

        foreach (var dsMapper in mappers)
        {
            var PrimaryDS = MapperService.CreateObsPDS(dsMapper, sourcDataPath);

            datasets.Add(PrimaryDS);
            
            //WriteDSToJSON(PrimaryDS, Path.Combine(outputDataPath, mapperFileName.Replace("_mapper", "").Replace(".csv","")));

        }
    }

    var consolidatedDatasets = MapperService.ConsolidateDatasets(datasets);
    foreach(var ds in consolidatedDatasets)
    {
       // var datatable = ds.TabulariseDataset();
        //TabularDataIO.WriteDataFile(Path.Combine(outputDataPath, datatable.TableName+".csv"), datatable);

       WriteDSToJSON(ds, outputDataPath);
    }
}

void WriteDSToJSON(PrimaryDataset? PrimaryDS, string outputDir)
{
    var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    string jsonString = JsonSerializer.Serialize(PrimaryDS, options);

    string JsonOutputFile = Path.Combine(outputDir,PrimaryDS?.DatasetDescriptor.Title + "_DATA.json");
    Directory.CreateDirectory(outputDir);
    
    File.WriteAllText(JsonOutputFile, jsonString);
}

void WriteDescriptorToJSON(ObservationDatasetDescriptor? obsDescriptor, string outputDir)
{
    var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
    string jsonString = JsonSerializer.Serialize(obsDescriptor, options);

    string JsonOutputFile = Path.Combine(outputDir, obsDescriptor.Title + "_Descriptor.json");
    Directory.CreateDirectory(outputDir);

    File.WriteAllText(JsonOutputFile, jsonString);
}

void CreateDatasetDescriptors()
{

    var mapperFileNames = ConfigurationManager.AppSettings.Get("MapperFileNames").Split(',');
    var descriptorsfullpath = ConfigurationManager.AppSettings.Get("DescriptorsDirectory");

    List<DatasetMapper> alldatasetmappers = new List<DatasetMapper>();
    List<DatasetDescriptor> allDescriptors = new();
    foreach (var mapper in mapperFileNames)
    {
        var mapperfullpath = Path.Combine(ConfigurationManager.AppSettings.Get("MappersDirectory"), mapper);


        TabularMapper tabularMapper = MapperService.ReadMappingFile(mapperfullpath);

        List<DatasetMapper> mappers = MapperService.ProcessTabularMapper(tabularMapper);

        allDescriptors.AddRange(MapperService.InitObsDescriptors(mappers));

        //List<DataTable> descriptors_dtList = MapperService.TabulariseDescriptors(descriptors);

        //foreach (var descriptorDT in descriptors_dtList)
        //{
        //    TabularDataIO.WriteDataFile(Path.Combine(descriptorsfullpath, descriptorDT.TableName.Replace(' ', '_') + '_'+mapper.Replace("mapper.csv","descriptor")+ ".csv"), descriptorDT);
        //}
    }

    var descriptorsByDomain = allDescriptors.GroupBy(p => p.Title);
    
    foreach (var descGroup in descriptorsByDomain)
    {
        var newDescriptor = (ObservationDatasetDescriptor)MapperService.GetConsolidatedDescriptor(descGroup.ToList());
        WriteDescriptorToJSON(newDescriptor, descriptorsfullpath);
    }
}




void CreateCDISCdescritptors()
{
    //DatasetTemplate template = MapperService.CreateCDISCtemplate(cdiscTemplatesDir, "AG-template-DS.csv", "AG-template-Vars.csv");

    var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10, DefaultIgnoreCondition= JsonIgnoreCondition.WhenWritingNull };
    //    string jsonString = JsonSerializer.Serialize(template, options);


    //    string JsonOutputFile = cdiscDescriptorDir + template.Id + "_DESCRIPTOR.json"; 
    //    File.WriteAllText(JsonOutputFile, jsonString);

    //###################
}



