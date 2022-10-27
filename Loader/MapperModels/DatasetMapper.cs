using System;
using PlatformTM.Core.Domain.Model.DatasetModel;
using System.Linq;
using Loader.MapperModels.SourceDataModels;
using PlatformTM.Core.Domain.Model.DatasetDescriptorTypes;

namespace PlatformTM.Models
{
    public class DatasetMapper
    {
        public string DatasetName { get; internal set; }
        public string StudyName { get; internal set; }
        public string SubjectVariableName { get; set; }
        public string VisitDateVariableName { get; set; }
        public string VisitVariableName { get; set; }
        public List<ObservationMapper> ObservationMappers { get; internal set; }

        public List<En>

        public List<SourceDataFile> SourceFiles;


        public DatasetMapper()
        {
            ObservationMappers = new List<ObservationMapper>();
        }


        //write a method here that will retrun ftom the obervation mappers, ther source data files that need
        // to be read, this iwll be used by the top method that will get data from original files and map them to the
        //observation models

        public List<SourceDataFile> InitializeSourceDataFiles(string srcDataPath)
        {
            var sourceFileNames = new HashSet<string>();
            List<SourceDataFile> sourceDataFiles = new();

            foreach (var propValueMapper in ObservationMappers
                .SelectMany(obsMapper => obsMapper.PropertyMappers
                .SelectMany(propMapper => propMapper.PropertyValueMappers)))
            {
                sourceFileNames.Add(propValueMapper.SourceFileName);
            }

            foreach (string filename in sourceFileNames)
            {
                SourceDataFile srcDataFile = new(filename, srcDataPath, SubjectVariableName);
                srcDataFile.SubjectIdVariableName = SubjectVariableName;
                sourceDataFiles.Add(srcDataFile);

                foreach ( var propValMapper in ObservationMappers
                    .SelectMany(obsMapper => obsMapper.PropertyMappers
                    .SelectMany(propMapper => propMapper.PropertyValueMappers))
                    )
                {
                    srcDataFile.DataVariables.Add(
                    new SourceDataVariable
                    {
                        Name = propValMapper.SourceVariableName,
                        Identifier = propValMapper.SourceVariableId,
                        Text = propValMapper.SourceVariableText
                        
                    });
                }
                    
                
            }
            SourceFiles = sourceDataFiles;

            return sourceDataFiles;
        }

        public List<string> GetDatasetSubjectIds()
        {
            List<string> subjectidSet = new();
            foreach(var srcFile in SourceFiles)
            {
                List<string>? Ids = srcFile.GetSubjectIds();
                if(Ids != null)
                    subjectidSet.AddRange(Ids);
            }
            return subjectidSet.ToHashSet().ToList();
            
        }

        public ObservationDatasetDescriptor InitObsDescriptor()
        {
            var obsDSdescriptor = new ObservationDatasetDescriptor
            {
                Title = DatasetName,
                SubjectIdentifierField = new IdentifierField()
                { Name = "SUBJID", Label = "Subject Identifier" },
                StudyIdentifierField = new IdentifierField()
                { Name = "STUDYID", Label = "Study Identifier" },
                FeatureNameField = new DesignationField()
                { Name = "OBSFEAT", Designation = "Name of Feature", Label = "Observed Feature" },
                FeaturePropertyNameField = new PropertyField()
                { Name = "FEATPROP", Label = "Feature Property Name"},
                FeaturePropertyValueField = new PropertyValueField()
                { Name = "FEATPROPVAL", Label = "Feature Property Value"}
            };



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

            var dsObsProperties = GetPropertyFields();
            foreach (var propertyName in dsObsProperties)
            {
                obsDSdescriptor.PropertyValueFields.Add(new PropertyValueField()
                {
                    Name = propertyName,
                    Label = propertyName,
                });
            }

            obsDSdescriptor.ObservationPropertyFields.Add(new DatasetField()
            {
                 Name = "VISIT", Label = "Visit Name"
            });

            obsDSdescriptor.ObservationPropertyFields.Add(new DatasetField()
            {
                Name = "DOV",
                Label = "Date Of Visit"
            });


            return obsDSdescriptor;
        }

        private List<string> GetPropertyFields()
        {
            HashSet<string> Properties = new();

            foreach (var propMapper in ObservationMappers
                .SelectMany(obsMapper => obsMapper.PropertyMappers).Where(p=>!p.IsFeatureProperty).OrderBy(pm => pm.Order))
            {
                Properties.Add(propMapper.PropertyName);
            }

   
            return Properties.ToList();
        }

        //public List<string> GetDatasetFeature()
        //{

        //}

        public string GetPropValueForSubject(string subjectId, PropertyMapper propertyMapper)
        {
            List<string> Values = new();
            foreach(var pvMapper in propertyMapper.PropertyValueMappers)
            {
                var srcDataFile = SourceFiles.Find(f => f.SourceFileName == pvMapper.SourceFileName);
                var srcDataRow = srcDataFile?.DataRows.Find(r => r.SubjectId == subjectId);
                var srcValue = srcDataRow?.DataRecord[pvMapper.SourceVariableId];

                var newVal = pvMapper.ValueExpression.EvaluateExpression(srcValue);
                if (newVal == "err")
                    throw new Exception("Error evaluating value expression for PVmapper: " + pvMapper.ToString());
                else
                    Values.Add(newVal);
            }
            if (Values.Count == 1) return Values[0];
            else
            {
                var arr = "[";
                foreach (var val in Values)
                {
                    arr += val + "|";
                }
                arr = arr.Substring(0, arr.Length - 1) + "]";
                return arr;
            }
        }


        public PrimaryDataset CreatePrimaryDataset()
        {
            PrimaryDataset PrimaryDataset = new() { };
         
            return null;
        }
    }
}

