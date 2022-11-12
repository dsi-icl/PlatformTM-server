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
using Loader.DB;


//const string outputDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Output/SMSsurvey/";
//const string srcDataDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Data/";
////const string mappersDir = "/Users/iemam/Box/UNICORN - Data FAIRification/STAGING/BT/Mappers/";
//const string cdiscTemplatesDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/";
//const string cdiscDescriptorDir = "/Users/iemam/Box/UNICORN - Data FAIRification/Dataset Templates/json_descriptors/";


var mapperFileNames = ConfigurationManager.AppSettings.Get("MapperFileNames").Split(',');
var sourcDataPath = ConfigurationManager.AppSettings.Get("SourceDataDirectory");
var outputDataPath = ConfigurationManager.AppSettings.Get("OutputDirectory");
var mappersPath = ConfigurationManager.AppSettings.Get("MappersDirectory");

int projectId;
int.TryParse(ConfigurationManager.AppSettings.Get("ProjectId"), out projectId);

FileService fileService = new FileService(projectId);



CreatePrimaryDatasets();
//CreateAndConsolidateDatasets
//CreateDatasetDescriptors();



void CreateAndConsolidateDatasets(string outputFolderName)
{

    var studyFolderName ="";
    var subFolderName = outputFolderName;
    var outputFolderPath = Path.Combine(outputDataPath, studyFolderName, subFolderName);

    List<PrimaryDataset> datasets = new();
    foreach (var mapperFileName in mapperFileNames)
    {
        //CHECK THESE EXIST FIRST BEFORE PROCEEDING
        studyFolderName = mapperFileName.Split('_')[0];
        
        var mapperFileFullPath = Path.Combine(mappersPath, studyFolderName, mapperFileName);
        var currSrcDataPath = Path.Combine(sourcDataPath, studyFolderName);

        

        // A new mapper service instance per mapper file
        MapperService mapper = new MapperService(projectId, currSrcDataPath, "", mapperFileFullPath);

        var dss= mapper.CreatePrimaryDataset();
        if (dss != null) datasets.AddRange(dss);
    }

    var consolidatedDatasets = MapperService.ConsolidateDatasets(datasets);
    
    foreach (var ds in consolidatedDatasets)
    {
        // var datatable = ds.TabulariseDataset();
        //TabularDataIO.WriteDataFile(Path.Combine(outputDataPath, datatable.TableName+".csv"), datatable);

        MapperService mapper = new MapperService(projectId, "", outputFolderPath, "");
        var fileInfo = mapper.WriteDSToJSON(ds);
        fileService.AddOrUpdateFile(studyFolderName, subFolderName, fileInfo);
    }
}

void CreatePrimaryDatasets()
{
 
    List<PrimaryDataset> datasets = new();
    
    foreach (var mapperFileName in mapperFileNames)
    {

        //CHECK THESE EXIST FIRST BEFORE PROCEEDING
        var studyFolderName = mapperFileName.Split('_')[0];
        var subFolderName = mapperFileName.Replace("_mapper", "").Replace(".csv", "");
        var mapperFileFullPath = Path.Combine(mappersPath, studyFolderName, mapperFileName);
        var currSrcDataPath = Path.Combine(sourcDataPath, studyFolderName);
        var outputFolderPath = Path.Combine(outputDataPath,studyFolderName,subFolderName); 

        // A new mapper service instance per mapper file
        MapperService mapper = new MapperService(projectId, currSrcDataPath, outputFolderPath, mapperFileFullPath);

        datasets = mapper.CreatePrimaryDataset();

        foreach (var dataset in datasets)
        {

            var fileInfo = mapper.WriteDSToJSON(dataset);
            fileService.AddOrUpdateFile(studyFolderName, subFolderName, fileInfo);
            
        }
    }
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

    //var mapper = new MapperService(projectId,sourcDataPath,outputDataPath);
    foreach (var mapperFileName in mapperFileNames)
    {

        //CHECK THESE EXIST FIRST BEFORE PROCEEDING
        var studyFolderName = mapperFileName.Split('_')[0];
        var subFolderName = mapperFileName.Replace("_mapper", "").Replace(".csv", "");
        var mapperFileFullPath = Path.Combine(mappersPath, studyFolderName, mapperFileName);
        var currSrcDataPath = Path.Combine(sourcDataPath, studyFolderName);
        var outputFolderPath = Path.Combine(outputDataPath, studyFolderName, subFolderName);

        // A new mapper service instance per mapper file
        MapperService mapper = new MapperService(projectId, currSrcDataPath, outputFolderPath, mapperFileFullPath);


        TabularMapper tabularMapper = mapper.ReadMappingFile(mapperFileFullPath);

        List<DatasetMapper> mappers = mapper.ProcessTabularMapper(tabularMapper);

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



