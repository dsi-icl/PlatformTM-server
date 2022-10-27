using System;
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

        internal static DatasetTemplate CreateCDISCtemplate(string cdiscTemplatesDir, string v1, string v2)
        {
            var DSinfo = TabularDataIO.ReadDataTable(Path.Combine(cdiscTemplatesDir, v1));
            var DSvarsInfo = TabularDataIO.ReadDataTable(Path.Combine(cdiscTemplatesDir, v2));

            var Template = new DatasetTemplate();
            Template.Domain = DSinfo.Rows[0]["Dataset Label"].ToString();
            Template.Class = DSinfo.Rows[0]["Class"].ToString();
            Template.Code = DSinfo.Rows[0]["Dataset Name"].ToString();
            Template.Id = DSinfo.Rows[0]["OID"].ToString();
            Template.Structure = DSinfo.Rows[0]["Structure"].ToString();
            Template.Description = DSinfo.Rows[0]["Description"].ToString();
            

            foreach(DataRow row in DSvarsInfo.Rows)
            {
                Template.Fields.Add(new DatasetTemplateField()
                {
                    Name = row["Variable Name"].ToString(),
                    Description = row["CDISC Notes"].ToString(),
                    DataType = row["Type"].ToString(),
                    Label = row["Variable Label"].ToString(),
                    Order = Int32.Parse(row["Variable Order"].ToString()),
                    Section = "Column",
                    AllowMultipleValues = false,
                    IsGeneric = false,
                    Id = "VS-SDTM-"+Template.Code+"-"+ row["Variable Name"].ToString(),
                    UsageId = GetUsageId(row["Core"].ToString()),
                    RoleId = GetRoleId(row["Role"].ToString()),
                    TemplateId = Template.Id
                }) ;
            }


            return Template;
        }

        private static string GetUsageId(string usageTerm)
        {
            return usageTerm switch
            {
                "Req" => ("CL-Compliance-T-1"),
                "Perm" => ("CL-Compliance-T-3"),
                "Exp" => ("CL-Compliance-T-2"),
                _ => "",
            };
        }

        private static string GetRoleId(string usageTerm)
        {
            return usageTerm switch
            {
                "Identifier" => ("CL-Role-T-1"),
                "Topic" => ("CL-Role-T-2"),
                "Record Qualifier" => ("CL-Role-T-3"),
                "Synonym Qualifier" => ("CL-Role-T-4"),
                "Variable Qualifier" => ("CL-Role-T-5"),
                "Timing" => ("CL-Role-T-6"),
                "Grouping Qualifier" => ("CL-Role-T-7"),
                "Result Qualifier" => ("CL-Role-T-8"),
                "Rule" => ("CL-Role-T-9"),
                _ => "",
            };
        }

        public static List<DatasetMapper> ProcessTabularMapper(TabularMapper tabularMapper)
        {
            List<DatasetMapper> DatasetMappers = new();
            //Dictionary<string, SourceDataFile> sourceDataFiles_map = new();

            Dictionary<string,List<TabularEntityMapper>> dsGroups = tabularMapper.GroupByDataset();
            foreach (var datasetGroup in dsGroups)
            {
                if (datasetGroup.Key == "")
                    continue;
                DatasetMapper datasetMapper = new(){DatasetName = datasetGroup.Key, StudyName= datasetGroup.Value?.FirstOrDefault()?.StudyName};
                datasetMapper.SubjectVariableName = tabularMapper.SubjectIdMapper?.SourceVariableName;
                datasetMapper.VisitDateVariableName = tabularMapper.StudyDateOfVisitMapper?.SourceVariableName;
                datasetMapper.VisitVariableName = tabularMapper.StudyVisitMappper?.SourceVariableName;
                DatasetMappers.Add(datasetMapper);

                Dictionary<string, List<TabularEntityMapper>> groupedByFeature = tabularMapper.GroupByObsFeature(datasetGroup.Value);

                foreach (var featGroup in groupedByFeature)
                {
                    if (featGroup.Value == null) continue;
                    var obsMapper = new ObservationMapper();
                    datasetMapper.ObservationMappers.Add(obsMapper);

                    var feature = featGroup.Value.First();
                    obsMapper.ObsFeatureValue.ValueString = feature.ObservedFeature;
                    obsMapper.Category = feature.ObservationCategory;
                    obsMapper.SubCategory = feature.ObservationSubcategory;
                    obsMapper.ObsGroupId = feature.ObservationGroupId;
                    obsMapper.IsDerived = feature.IsDerived;
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
                                    IsFeatureProperty = tabularObsMap.IsFeatureProperty
                                };

                                obsMapper.PropertyMappers.Add(propMapper);
                            }

                            propMapper.PropertyValueMappers.Add(new PropertyValueMapper(tabularPropMap.PropertyValue)
                            {
                                SourceFileName = tabularObsMap.SourceFileName,
                                SourceVariableId = tabularObsMap.SourceVariableId,
                                SourceVariableName = tabularObsMap.SourceVariableName,
                                SourceVariableText = tabularObsMap.SourceVariableText
                            });
                        }
                    }

                        //Dictionary<string, List<TabularEntityMapper>> groupedByProperty = tabularMapper.GroupByObservedProperty(featGroup.Value);
                }

                //    //UNIQUE OBSERAVTION INSTANCES PER DATASET
                //    //foreach (var map in groupedByFeature)
                //    {


                //    }
                //    //var uniq_observations = group.GroupBy(o => new { feat = o.Field<string>("OBSERVED_FEATURE"), seq = o.Field<string>("OBSERVATION_SEQ") });
                //    //foreach (var obsRows in uniq_observations)


             }


            //    foreach (var varMapper in tabularMapper.EntityMappers)
            //{
            //    if (varMapper.IsSkipped)
            //        continue;
            //    if (!DatasetMapper_map.TryGetValue(varMapper.DatasetName, out DatasetMapper DatasetMapper))
            //    {
            //        DatasetMapper = new DatasetMapper()
            //        {
            //            DatasetName = varMapper.DatasetName,
            //            StudyName = varMapper.StudyName
            //        };
            //        DatasetMapper_map.Add(varMapper.DatasetName,DatasetMapper);
            //    }

                //WHAT IS THE OBS MAPPER REPRESENFINT?
                //It's representing the observation
                //so what is an observation?
                //is it the one instance of observation feature and one property but with multi-valued properties?
                // or is it the one observation feaure with ALL properties.
                //For now I want to keep it that all properties are group with the obsFeature


                //if (!sourceDataFiles_map.TryGetValue(varMapper.SourceFileName, out SourceDataFile sourceDataFile))
                //{
                //    sourceDataFile = new(varMapper.SourceFileName, "");

                //    sourceDataFiles_map.Add(varMapper.SourceFileName, sourceDataFile);
                //}

                //var filepath = Path.Combine(SourceFilesPath, file.SourceDataFileName);
                //     DataTable dt = ReadDataTable(filepath);
                //     Program.ImportSourceData(file,dt);

                //This is iterating over the properties that are in the same row / mapper record

                //var obsMapper = new ObservationMapper()
                //;
                //obsMapper.ObsFeatureValue.ValueString = varMapper.ObservedFeature;
                //obsMapper.Category = varMapper.ObservationCategory;
                //obsMapper.ObsGroupId = varMapper.ObservationGroupId;
                //obsMapper.IsDerived = varMapper.IsDerived;

                //DatasetMapper.ObservationMappers.Add(obsMapper);

                //foreach (var propMap in varMapper.PropertyMappers)
                //{
                    

                    //create a method that gets value if refernce instead of string
                    

                    //PropertyMappers
                    //obsMapper.PropertyMapper.PropertyName = propMap.PropertyName;
                    
                    //obsMapper.PropertyMapper.PropertyValueMappers = (new PropertyValueMapper(propMap.PropertyValue)
                   // {
                    //    Unit = propMap.PropertyValueUnit
                    //});
              //  }
                //add studyId


          //  }
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

        //public static ObservationDatasetDescriptor InitObsDescriptor(DatasetMapper dsMapper)
        //{
        //    var obsDSdescriptor = new ObservationDatasetDescriptor
        //    {
        //        Title = dsMapper.DatasetName,
        //        SubjectIdentifierField = new IdentifierField()
        //        { Name = "SUBJID", Label = "Subject Identifier" },
        //        StudyIdentifierField = new IdentifierField()
        //        { Name = "STUDYID", Label = "Study Identifier" },
        //        FeatureNameField = new DesignationField()
        //        { Name = "OBSFEAT", Designation = "Name of Feature", Label = "Observed Feature" }
        //    };
        //    obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
        //    {
        //        Name = "FEATCAT",
        //        Label = "Feature Category",
        //        Order = 1
        //    });

        //    obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
        //    {
        //        Name = "FEATSCAT",
        //        Label = "Feature Subcategory",
        //        Order = 2
        //    });

        //    var dsObsProperties = dsMapper.GetPropertyFields();
        //    foreach (var prop in dsObsProperties)
        //    {
        //        obsDSdescriptor.PropertyValueFields.Add(new PropertyValueField()
        //        {
        //            Name = prop,
        //            Label = prop
        //        });
        //    }
        //    return obsDSdescriptor;
        //}

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


                foreach (var oMapper in datasetMapper.ObservationMappers)
                {
                    var datasetRecord = new DatasetRecord();
                    PrimaryDataset.DataRecords.Add(datasetRecord);


                    //SubjectId
                    datasetRecord[datasetDescriptor.SubjectIdentifierField?.Name] = subjectId;

                    //StudyName
                    datasetRecord[datasetDescriptor.StudyIdentifierField?.Name] = datasetMapper.StudyName;

                    //FeatureCategory
                    datasetRecord[datasetDescriptor.GetClassifierField(1)?.Name] = oMapper.Category;

                    //FeatureSubCategory
                    datasetRecord[datasetDescriptor.GetClassifierField(2)?.Name] = oMapper.SubCategory;

                    //FeatureName
                    datasetRecord[datasetDescriptor.FeatureNameField.Name] = oMapper.ObsFeatureValue.ValueString;


                    //FeatureProp
                   // datasetRecord[datasetDescriptor.FeaturePropertyNameField.Name] = oMapper.ObsFeatureValue.ValueString;

                    //FeaturePropValue
                    //datasetRecord[datasetDescriptor.FeaturePropertyValueField.Name] = oMapper.ObsFeatureValue.ValueString;

                    //ONE datasetrecord per subject here ... so

                    foreach (var field in datasetDescriptor.PropertyValueFields)
                    {
                        var propMapper = oMapper.GetPropertyMapper(field.Label);
                        if (propMapper != null)
                            datasetRecord[field.Label] = datasetMapper.GetPropValueForSubject(subjectId, propMapper);
                        else
                            datasetRecord[field.Label] = "";
                    }



                    //foreach (var pMapper in oMapper.PropertyMappers)
                    //{

                    //    //TEMP
                    //    //if (pMapper.IsDerived) continue;

                    //    //FOR EACH pMapper here I want to retrieve the resepective value for ALL SUBJECTS from the source file and
                    //    //AWEL BOM AHEIH! What is a unique DATASET RECORD? ONE PER SUBJECT PER FEATURE
                    //    //The uniqeness of observation feature should be already established in the listed observation mappers in each dataset mapper
                    //    //therefore for each observed feature instance a new subject record will be created 
                    //    pMapper.
                       

                    //    foreach (var pvMapper in pMapper.PropertyValueMappers)
                    //    {
                    //        var sourceDataFile = sourceDataFiles.FirstOrDefault(f => f.SourceFileName == pvMapper.SourceFileName);
                    //    }

                    //    var dataValue = subjectRecord.DataRecord[pvMapper.SourceVariableId].ToString();

                    //    //    foreach (SubjectDataRow subjectRecord in sourceDataFile.SubjectDataRows)

                    //    //    {
                    //    //        //ana hena 3awez ageeb el subject data menel files elli el dataset dih 3awza data menha
                    //    //        //ya3ni el subject data hena momken tigi men kaza data file
                    //    //        //yeb2a b2a 2a3temed 3ala kol line 3ashan a retrieve el datafile beta3o we sa3etha a iterate over el subject data elli fil file dah

                    //    //        var obs = new Observation
                    //    //        {
                    //    //            Feature = oMapper.ObsFeature,
                    //    //            Category = oMapper.Category,
                    //    //            Property = pMapper.Property,
                    //    //            IsMultiValued = pMapper.IsMultiValued,
                    //    //            SubjectId = subjectRecord.SubjectId
                    //    //        };
                    //    //        Observations.Add(obs);


                    //    //        foreach (var pvMapper in pMapper.PropertyValueMappers)
                    //    //        {
                    //    //            if (pvMapper.ValueString == "$VAL")
                    //    //            {
                    //    //                //string val = subjectVariableData.DataRow[pMapper.SourceVariableId].ToString();
                    //    //                var dataValue = subjectRecord.DataRecord[pvMapper.SourceVariableId].ToString();
                    //    //                if (dataValue != "-10")
                    //    //                    obs.PropertyValues.Add(dataValue);
                    //    //                if (dataValue == "-9")
                    //    //                    obs.NotDone = true;


                    //    //            }
                    //    //            else if (pvMapper.HasDictionary)
                    //    //            {
                    //    //                var dataValue = subjectRecord.DataRecord[pvMapper.SourceVariableId].ToString();

                    //    //                if (dataValue == "-9")
                    //    //                    obs.NotDone = true;

                    //    //                if (pvMapper.DataDictionary.TryGetValue(dataValue, out string pv))
                    //    //                {
                    //    //                    if (!pv.Contains("$SKIP"))
                    //    //                        obs.PropertyValues.Add(pv);
                    //    //                }

                    //    //                else if (pvMapper.DataDictionary.ContainsKey("$VAL"))
                    //    //                    obs.PropertyValues.Add(dataValue);

                    //    //            }
                    //    //            else if (pvMapper.IsExpression && pMapper.IsDerived)
                    //    //            {
                    //    //                if (pvMapper.Function == "ANY")
                    //    //                {

                    //    //                    List<string> values = pvMapper.SourceVariables.Select(p => subjectRecord.DataRecord[p]).ToList();
                    //    //                    if (values.All(v => v == "-9"))
                    //    //                    {
                    //    //                        obs.NotDone = true;
                    //    //                        obs.PropertyValues.Add("Subject Did Not Attend");
                    //    //                        continue;
                    //    //                    }

                    //    //                    if (values.Contains(pvMapper.ConditionValue))
                    //    //                        obs.PropertyValues.Add(pvMapper.TruthyValue);
                    //    //                    else
                    //    //                        obs.PropertyValues.Add(pvMapper.FalsyValue);
                    //    //                }
                    //    //            }
                    //    //            else
                    //    //            {
                    //    //                obs.PropertyValues.Add(pvMapper.ValueString);
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}
                    //    // }
                    //    // return Observations;


                    //}
                }


            }







            //List<Observation> Observations = new();
            //ITERATE OVER THE OBSERVATION MAPPERS, AND FOR EACH OBSERVATION MAPPER ITERATE OVER THE PROPERTIES

            //foreach (DataRow row in dataTable.Rows)
            //{
            //    var sourceFile = sourceDataFiles[row.Field<string>("SOURCE_FILE")];

            //    sourceFile.DataVariables.Add(
            //        new SourceVariable
            //        {
            //            ColumnName = row.Field<string>("VARIABLE_NAME"),
            //            Identifier = row.Field<string>("VARIABLE_ID"),
            //            IsDerived = row[6].ToString().ToUpper().Equals("$DERIVED")
            //        });
            //}



            //foreach(var obsM in datasetMapper.ObservationMappers)
            //{
            //    var obs = new Observation();
            //    var feature = new Feature();
            //    feature.Name = obsM.GetFeatureName();

            //    //obs.ObservedPhenomenon.ObservedFeature
            //}

            return PrimaryDataset;

        }

        

    }
}

