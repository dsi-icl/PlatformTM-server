﻿using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;
using Loader;
using Loader.MapperModels.SourceDataModels;
using Loader.MapperModels.TabularMapperModels;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Core.Domain.Model.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.MapperModels;
using PlatformTM.MapperModels.TabularMapperModels;
using PlatformTM.Models;

namespace PlatformTM
{
    public class MapperService
    {
        private readonly IRepository<PrimaryDataset, int> _PDSRepository;
        private static int val;

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
                        SourceVariableIsMultiVal = csv.GetField<string>("SRC_VARIABLE_ISMULTIVAL").ToUpper().Equals("Y"),
                        MappedToEntity = csv.GetField<string>("MAP_TO"),
                        DatasetName = csv.GetField<string>("DATASET"),
                        ObservationCategory = csv.GetField<string>("OBS_CAT"),
                        ObservationSubcategory = csv.GetField<string>("OBS_SUBCAT"),
                        ObservedFeature = csv.GetField<string>("OBS_FEATURE"),
                        ObservationGroupId = csv.GetField<string>("OBS_GRP_ID"),
                        IsDerived = csv.GetField<string>("MAP_TO").ToUpper().Equals("$DERIVED"),
                        IsSkipped = csv.GetField<string>("MAP_TO").ToUpper().Equals("$SKIP"),
                        IsFeatureProperty = csv.GetField<string>("MAP_TO").ToUpper().Equals("$FEATPROP"),
                        

                    };

                    Regex rgx = new("(OBS_PROPERTY_)\\d{1}\\Z");

                    foreach (var col in csv.HeaderRecord)
                        if (rgx.IsMatch(col.ToString()))
                        {
                            if (csv[col].ToString() == "") continue;
                            record.PropertyMappers.Add(new TabularPropertyMapper()
                            {
                                PropertyName = csv.GetField<string>(col),
                                PropertyValue = csv.GetField<string>(col + "_VAL"),
                                PropertyValueUnit = csv.GetField<string>(col + "_VAL_UNIT"),
                                PropertyOrder = int.TryParse(csv.GetField<string>(col + "_ORDER"), out int val) ? val : 0 
                        });

                        }
                    tabularMapper.EntityMappers.Add(record);
                }
            }
            
            return tabularMapper;
        }

        

        public static List<DatasetMapper> ProcessTabularMapper(TabularMapper tabularMapper)
        {
            List<DatasetMapper> DatasetMappers = new();

            //GROUP BY DATASET
            Dictionary<string,List<TabularEntityMapper>> dsGroups = tabularMapper.GroupByDataset();

            //PROCESS EACH DATASET GROUP AND CREATE A DATASETMAPPER FOR EACH DATASET
            foreach (var datasetGroup in dsGroups)
            {
                if (datasetGroup.Key == "")
                    continue;

                DatasetMapper datasetMapper = new(){DatasetName = datasetGroup.Key, StudyName= datasetGroup.Value?.FirstOrDefault()?.StudyName};
                DatasetMappers.Add(datasetMapper);
                datasetMapper.SubjectVariableName = tabularMapper.GetSubjectVariableName();

                //Get All Source Data Variables relevant to this dataset
                foreach(var tabularMapperRow in datasetGroup.Value)
                {
                    datasetMapper.SourceDataVariables.Add(new SourceDataVariable
                    {
                        Name = tabularMapperRow.SourceVariableName,
                        Identifier = tabularMapperRow.SourceVariableId,
                        Text = tabularMapperRow.SourceVariableText,
                        SourceFileName = tabularMapperRow.SourceFileName,
                        IsMultiVal = tabularMapperRow.SourceVariableIsMultiVal
                    });
                }


                //Process Property Fields that are applicable to all observations (e.g. visit, visit date...)
                var otherMappers = tabularMapper.GetNonObsEntityMappers();
                foreach(var entityMapper in otherMappers)
                {


                    //Create PropertMapper
                    var tabPropMapper = entityMapper?.PropertyMappers.FirstOrDefault();
                    var PropMapper = new PropertyMapper(tabPropMapper.PropertyName);
                    PropMapper.PropertyValueMappers.Add(
                    new PropertyValueMapper(tabPropMapper.PropertyValue)
                    {
                        SourceFileName = entityMapper.SourceFileName,
                        SourceVariableId = entityMapper.SourceVariableId,
                    });
                    datasetMapper.PropertyMappers.Add(PropMapper);


                    //Add Corresponding Source Data Variable to the list of Dataset SourceDataVariables
                    datasetMapper.SourceDataVariables.Add(new SourceDataVariable
                    {
                        Name = entityMapper.SourceVariableName,
                        Identifier = entityMapper.SourceVariableId,
                        Text = entityMapper.SourceVariableText,
                        SourceFileName = entityMapper.SourceFileName
                    });
                }


                //NEXT PROCESS OBSERVATION FEATURES AND CREATE OBSERVATIONMAPPER FOR EACH FEATURE_GROUP
                Dictionary<string, List<TabularEntityMapper>> groupedByFeature = tabularMapper.GroupByObsFeature(datasetGroup.Value);

                foreach (var featGroup in groupedByFeature)
                {
                    if (featGroup.Value == null) continue;
                    var feature = featGroup.Value.First();

                    var obsMapper = new ObservationMapper(feature.ObservedFeature);
                    obsMapper.Category = feature.ObservationCategory;
                    obsMapper.SubCategory = feature.ObservationSubcategory;
                    obsMapper.ObsGroupId = feature.ObservationGroupId;
                    obsMapper.IsDerived = feature.IsDerived;

                    obsMapper.SourceFileName = feature.SourceFileName;


                    datasetMapper.ObservationMappers.Add(obsMapper);

                    foreach (var tabularObsMap in featGroup.Value)
                    {
                        foreach (var tabularPropMap in tabularObsMap.PropertyMappers)
                        {
                            if (tabularPropMap.PropertyName == null|| tabularPropMap.PropertyValue == null) continue;

                            var propMapper = obsMapper.GetPropertyMapper(tabularPropMap.PropertyName);
                            if(propMapper == null)
                            {
                                propMapper = new PropertyMapper(tabularPropMap.PropertyName)
                                {
                                    IsDerived = tabularObsMap.IsDerived,
                                    Order = tabularPropMap.PropertyOrder,
                                    Unit = tabularPropMap.PropertyValueUnit,
                                    IsFeatureProperty = tabularObsMap.IsFeatureProperty
                                };

                                obsMapper.PropertyMappers.Add(propMapper);
                            }

                            propMapper.PropertyValueMappers.Add(new PropertyValueMapper(tabularPropMap.PropertyValue)
                            {
                                SourceFileName = tabularObsMap.SourceFileName,
                                SourceVariableId = tabularObsMap.SourceVariableId,
                                //SourceVariableName = tabularObsMap.SourceVariableName,
                                //SourceVariableText = tabularObsMap.SourceVariableText
                            });
                        }
                    }
                }
             }
            return DatasetMappers;
        }

        public static List<ObservationDatasetDescriptor> InitObsDescriptors(List<DatasetMapper> datasetMappers)
        {
            List<ObservationDatasetDescriptor> descriptors = new();

            foreach (var dsMapper in datasetMappers)
            {
                descriptors.Add(dsMapper.InitObsDescriptor());
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

                foreach (var timeField in descriptor.ObservationPropertyFields)
                {
                    descDT.Rows.Add(timeField.Name, "", "", "PropertyValueField", "", "");
                }
            }
            return descriptorsList;
        }

        public static PrimaryDataset? CreateObsDatasets(DatasetMapper datasetMapper, string srcDataPath)
        {

            //Initialise SourceDataFiles from DatasetMapper to init Soure Data Variables
            var sourceDataFiles = datasetMapper.InitializeSourceDataFiles(srcDataPath);


            //Read actual source data files store in SrcDataRows
            foreach(var srcFile in sourceDataFiles)
            {
                srcFile.ReadSourceDataFile();
            }

            ObservationDatasetDescriptor datasetDescriptor = datasetMapper.InitObsDescriptor();

            var DatasetSubjectIds = datasetMapper.GetDatasetSubjectIds();

            if (DatasetSubjectIds.Count == 0)
                return null;

            //I HAVE  ONE PROBLEM NOW AND THAT IS IF I USE THE BMO CLASSES AS THEY ARE NOW, THERE IS NO ENTITY/CLASS THAT GROUPS ALL OBSERVATIONS RELATED TO THE SAME FEATURE
            //IE ALL OBSERVED PHENOMENONS OBSERVED FOR A SINGLE OBSERVED FEATURE (THAT IS WHAT WOULD BE ONE DATASET RECORD IN THE PRIMARY DATASET

            PrimaryDataset PrimaryDataset = new();
            PrimaryDataset.DatasetDescriptor = datasetDescriptor;
             
            foreach (var subjectId in DatasetSubjectIds)
            {
                //THIS NEEDS UPDATING when there are multiple source files
                var subjectSrcDataRows = sourceDataFiles[0]?.DataRows.FindAll(r => r.SubjectId == subjectId);
                foreach (var subjectDataRow in subjectSrcDataRows)
                {

                    foreach (var oMapper in datasetMapper.ObservationMappers)
                    {



                        var datasetRecord = new DatasetRecord(); //THIS WOULD bE ANOTHER TOP LEVEL RECORDSPERSUBJECT ID


                        //SubjectId
                        datasetRecord[datasetDescriptor.SubjectIdentifierField?.Name] = subjectId;

                        //StudyName
                        datasetRecord[datasetDescriptor.StudyIdentifierField?.Name] = datasetMapper.StudyName;

                        //Dataset Domain
                        datasetRecord[datasetDescriptor.GetClassifierField(1)?.Name] = datasetMapper.DatasetName;

                        //FeatureCategory
                        datasetRecord[datasetDescriptor.GetClassifierField(2)?.Name] = oMapper.Category;

                        //FeatureSubCategory
                        datasetRecord[datasetDescriptor.GetClassifierField(3)?.Name] = oMapper.SubCategory;

                        //FeatureName
                        var featureName = datasetMapper.EvaluateFeatureMapper(subjectDataRow, oMapper);
                        if (featureName != "")
                            datasetRecord[datasetDescriptor.FeatureNameField.Name] = featureName;
                        else
                            continue;


                        if (datasetMapper.HasFeatureProperties())
                        {
                            var featPropMapper = oMapper.GetFeaturePropertyMapper();
                            //FeatureProp
                            datasetRecord[datasetDescriptor.FeaturePropertyNameField.Name] = featPropMapper != null ? featPropMapper.PropertyName : "";

                            //FeaturePropValue
                            datasetRecord[datasetDescriptor.FeaturePropertyValueField.Name] = featPropMapper != null ? datasetMapper.EvaluatePropertyValueMapper(subjectDataRow, featPropMapper) : "";

                        }


                        //ObservedPropertyValues
                        foreach (var field in datasetDescriptor.PropertyValueFields)
                        {
                            var propMapper = oMapper.GetPropertyMapper(field.Name); //THIS SHOULD COME FROM A MAP THAT LINKS PROPMAPPERS to DATASET FIELDS
                            if (propMapper != null)
                            {
                                datasetRecord[field.Name] = datasetMapper.EvaluatePropertyValueMapper(subjectDataRow, propMapper);
                                if (propMapper.HasUnit())
                                    datasetRecord["UNIT[" + field.Name + "]"] = propMapper.Unit;
                            }
                            else
                                datasetRecord[field.Name] = "";
                        }

                        //Dataset Wide Observation Properties
                        foreach (var field in datasetDescriptor.ObservationPropertyFields)
                        {
                            var propMapper = datasetMapper.GetPropertyMapper(field.Name); //THIS SHOULD COME FROM A MAP THAT LINKS PROPMAPPERS to DATASET FIELDS
                            datasetRecord[field?.Name] = datasetMapper.EvaluatePropertyValueMapper(subjectDataRow, propMapper);
                        }

                        PrimaryDataset.DataRecords.Add(datasetRecord);
                    }

                }

            }

            return PrimaryDataset;

        }

        

    }
}

