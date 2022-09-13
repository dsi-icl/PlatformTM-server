using System;
using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using Loader.MapperModels.SourceDataModels;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.MapperModels;
using PlatformTM.MapperModels.TabularMapperModels;
using PlatformTM.Models;

namespace PlatformTM
{
    public class MapperService
    {
        private readonly IRepository<PrimaryDataset, int> _PDSRepository;
        public MapperService()
        {
            
    }

        public static List<TabularEntityMapper> ReadMappingFile(string MapperFilePath)
        {

            var tabularMapper = new TabularMapper();

            var records = new List<TabularEntityMapper>();
            using (var reader = new StreamReader(MapperFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {

                csv.Read();
                csv.ReadHeader();
                while (csv.Read())
                {
                    var record = new TabularEntityMapper
                    {
                        StudyName = csv.GetField<string>("STUDY_NAME"),
                        SourceFileName = csv.GetField<string>("SRC_FILE"),
                        SourceVariableId = csv.GetField<string>("SRC_VARIABLE_ID"),
                        SourceVariableName = csv.GetField<string>("SRC_VARIABLE_NAME"),
                        SourceVariableText = csv.GetField<string>("SRC_VARIABLE_TEXT"),
                        MappedToEntity = csv.GetField<string>("MAP_TO"),
                        DatasetName = csv.GetField<string>("DATASET"),
                        ObservationCategory = csv.GetField<string>("OBS_CAT"),
                        ObservationSubcategory = csv.GetField<string>("OBS_SUBCAT"),
                        ObservedFeature = csv.GetField<string>("OBS_FEATURE"),
                        ObservationGroupId = csv.GetField<string>("OBS_GRP_ID"),
                        IsDerived = csv.GetField<string>("MAP_TO").ToUpper().Equals("$DERIVED"),
                        IsSkipped = csv.GetField<string>("MAP_TO").ToUpper().Equals("$SKIP"),

                    };

                    Regex rgx = new("(OBS_PROPERTY_)\\d{1}\\Z");

                    foreach (var col in csv.HeaderRecord)
                        if (rgx.IsMatch(col.ToString()))
                        {
                            if (csv[col].ToString() == "") continue;
                            record.MappedProperties.Add(new TabularEntityMapper.TabularPropMapper()
                            {
                                PropertyName = csv.GetField<string>(col),
                                PropertyValue = csv.GetField<string>(col + "_VAL"),
                                PropertyValueUnit = csv.GetField<string>(col + "_VAL_UNIT")
                            });

                        }
                    tabularMapper.ObsMappers.Add(record);
                }
            }
            return records;
        }

        public static Dictionary<string, DatasetMapper> ProcessTabularMapper(List<TabularEntityMapper> varMappers)
        {
            Dictionary<string, DatasetMapper> DatasetMapper_map = new();
            Dictionary<string, SourceDataFile> sourceDataFiles_map = new();

            

            foreach (var varMapper in varMappers)
            {
                if (varMapper.IsSkipped)
                    continue;
                if (!DatasetMapper_map.TryGetValue(varMapper.DatasetName, out DatasetMapper DatasetMapper))
                {
                    DatasetMapper = new DatasetMapper()
                    {
                        DatasetName = varMapper.DatasetName,
                        StudyName = varMapper.StudyName
                    };
                    DatasetMapper_map.Add(varMapper.DatasetName,DatasetMapper);
                }

                if (!sourceDataFiles_map.TryGetValue(varMapper.SourceFileName, out SourceDataFile sourceDataFile))
                {
                    sourceDataFile = new(varMapper.SourceFileName, "");
                  
                    sourceDataFiles_map.Add(varMapper.SourceFileName, sourceDataFile);
                }

                //var filepath = Path.Combine(SourceFilesPath, file.SourceDataFileName);
                //     DataTable dt = ReadDataTable(filepath);
                //     Program.ImportSourceData(file,dt);

                foreach (var prop in varMapper.MappedProperties)
                {
                    var obsMapper = new ObservationMapper()
                    {
                        SourceFileName = varMapper.SourceFileName,
                        SourceVariableId = varMapper.SourceVariableId,
                        SourceVariableName = varMapper.SourceVariableName,
                        SourceVariableText = varMapper.SourceVariableText,

                    };
                    DatasetMapper.ObservationMappers.Add(obsMapper);

                    obsMapper.ObsFeatureValue.ValueString = varMapper.ObservedFeature;
                    obsMapper.Category = varMapper.ObservationCategory;
                    obsMapper.ObsGroupId = varMapper.ObservationGroupId;

                    obsMapper.PropertyMapper.PropertyName = prop.PropertyName;
                    obsMapper.IsDerived = varMapper.IsDerived;

                    obsMapper.PropertyMapper.PropertyValueMappers.Add(new PropertyValueMapper(prop.PropertyValue)
                    {
                        Unit = prop.PropertyValueUnit
                    });
                }


            }
            return DatasetMapper_map;
        }

        public static void CreateObsDatasets(Dictionary<string,DatasetMapper> datasetMappers)
        {
            foreach(var dsMapper in datasetMappers.Values)
            {
                //new dataset

                CreateDataset();

                foreach(var obsM in dsMapper.ObservationMappers)
                {
                    var obs = new Observation();
                    var feature = new Feature();
                    feature.Name = obsM.GetFeatureName();

                    obs.ObservedPhenomenon.ObservedFeature
                }
            }
        }

        private static void CreateDataset()
        {
            PrimaryDataset primaryDataset = new()
            {
                ProjectId = ,
                StudyId = ,
                n
            }
        }
    }
}

