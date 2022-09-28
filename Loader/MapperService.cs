using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using Loader.MapperModels.SourceDataModels;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Core.Domain.Model.DatasetDescriptorTypes;
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

        public static TabularMapper ReadMappingFile(string MapperFilePath)
        {

            var tabularMapper = new TabularMapper();

            //var records = new List<TabularEntityMapper>();
            
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
                            record.ObservedPropertyMappers.Add(new TabularPropertyMapper()
                            {
                                PropertyName = csv.GetField<string>(col),
                                PropertyValue = csv.GetField<string>(col + "_VAL"),
                                PropertyValueUnit = csv.GetField<string>(col + "_VAL_UNIT")
                            });

                        }
                    tabularMapper.EntityMappers.Add(record);
                }
            }
            
            return tabularMapper;
        }

        public static Dictionary<string, DatasetMapper> ProcessTabularMapper(TabularMapper tabularMapper)
        {
            Dictionary<string, DatasetMapper> DatasetMapper_map = new();
            //Dictionary<string, SourceDataFile> sourceDataFiles_map = new();

            

            foreach (var varMapper in tabularMapper.EntityMappers)
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

                //if (!sourceDataFiles_map.TryGetValue(varMapper.SourceFileName, out SourceDataFile sourceDataFile))
                //{
                //    sourceDataFile = new(varMapper.SourceFileName, "");
                  
                //    sourceDataFiles_map.Add(varMapper.SourceFileName, sourceDataFile);
                //}

                //var filepath = Path.Combine(SourceFilesPath, file.SourceDataFileName);
                //     DataTable dt = ReadDataTable(filepath);
                //     Program.ImportSourceData(file,dt);

                //This is iterating over the properties that are in the same row / mapper record
                foreach (var prop in varMapper.ObservedPropertyMappers)
                {
                    var obsMapper = new ObservationMapper()
                    {
                        SourceFileName = varMapper.SourceFileName,
                        SourceVariableId = varMapper.SourceVariableId,
                        SourceVariableName = varMapper.SourceVariableName,
                        SourceVariableText = varMapper.SourceVariableText,

                    };
                    DatasetMapper.ObservationMappers.Add(obsMapper);

                    //create a method that gets value if refernce instead of string
                    obsMapper.ObsFeatureValue.ValueString = varMapper.ObservedFeature;
                    obsMapper.Category = varMapper.ObservationCategory;
                    obsMapper.ObsGroupId = varMapper.ObservationGroupId;
                    obsMapper.IsDerived = varMapper.IsDerived;

                    //PropertyMappers
                    obsMapper.PropertyMapper.PropertyName = prop.PropertyName;
                    
                    obsMapper.PropertyMapper.PropertyValueMappers.Add(new PropertyValueMapper(prop.PropertyValue)
                    {
                        Unit = prop.PropertyValueUnit
                    });
                }
                //add studyId


            }
            return DatasetMapper_map;
        }

        public static List<ObservationDatasetDescriptor> InitObsDescriptors(List<DatasetMapper> datasetMappers)
        {
            List<ObservationDatasetDescriptor> descriptors = new();

            foreach (var dsMapper in datasetMappers)
            {
                var obsDSdescriptor = new ObservationDatasetDescriptor();
                descriptors.Add(obsDSdescriptor);

                obsDSdescriptor.Title =  dsMapper.DatasetName;
                obsDSdescriptor.SubjectIdentifierField = new IdentifierField()
                    { Name= "SUBJID", Label="Subject Identifier"};
                obsDSdescriptor.StudyIdentifierField = new IdentifierField()
                    { Name = "STUDYID", Label = "Study Identifier" };
                obsDSdescriptor.FeatureNameField = new DesignationField()
                    { Name = "OBSFEAT", Designation="Name of Feature", Label = "Observed Feature" };
                obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
                {
                    Name = "FEATCAT",
                    Label = "Feature Category",
                    Order = 1
                });

                obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
                {
                    Name = "FEATSCAT",
                    Label = "Feature Subcategory",
                    Order = 2
                });

                var dsObsProperties = dsMapper.GetPropertyFields();
                foreach(var prop in dsObsProperties)
                {
                    obsDSdescriptor.PropertyValueFields.Add(new PropertyValueField()
                    {
                        Name = prop,
                        Label = prop
                    }) ;
                }
            }

                return descriptors;
        }

        public static List<DataTable> TabulariseDescriptors(List<ObservationDatasetDescriptor> descriptors)
        {
            var descriptorsList = new List<DataTable>();
            foreach (ObservationDatasetDescriptor descriptor in descriptors)
            {
                var descDT = new DataTable();
                descDT.TableName = descriptor.Title;

                descriptorsList.Add(descDT);

                descDT.Columns.Add("FIELD_NAME");
                descDT.Columns.Add("FIELD_LABEL");
                descDT.Columns.Add("FIELD_DESCRIPTION");
                descDT.Columns.Add("FIELD_TYPE");
                descDT.Columns.Add("FIELD_NAME_TERMID");
                descDT.Columns.Add("FIELD_NAME_TERMREF");

                descDT.Rows.Add(descriptor.StudyIdentifierField.Name, descriptor.StudyIdentifierField.Label, "", "IdentifierField","","");
                descDT.Rows.Add(descriptor.SubjectIdentifierField.Name, descriptor.SubjectIdentifierField.Label, "", "IdentifierField", "", "");

                descriptor.ClassifierFields.Sort((a, b) => (a.Order.CompareTo(b.Order)));
                foreach (var classifierField in descriptor.ClassifierFields)
                {
                    descDT.Rows.Add(classifierField.Name, classifierField.Label, "", "ClassifierFieldType", "", "");
                }

                descDT.Rows.Add(descriptor.FeatureNameField.Name, descriptor.FeatureNameField.Label, "", "DesignationField", "", "");

                foreach (var propertyValueField in descriptor.PropertyValueFields)
                {
                    descDT.Rows.Add(propertyValueField.Name, "", "", "PropertyValueField", "", "");
                }
            }
            return descriptorsList;
        }

        public static void CreateObsDatasets(Dictionary<string,DatasetMapper> datasetMappers)
        {
            foreach(var dsMapper in datasetMappers.Values)
            {
                //new dataset

               // CreateDataset();

                foreach(var obsM in dsMapper.ObservationMappers)
                {
                    var obs = new Observation();
                    var feature = new Feature();
                    feature.Name = obsM.GetFeatureName();

                    //obs.ObservedPhenomenon.ObservedFeature
                }
            }
        }

        
    }
}

