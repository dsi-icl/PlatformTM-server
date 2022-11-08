using System;
using PlatformTM.Core.Domain.Model.DatasetModel;
using System.Linq;
using Loader.MapperModels.SourceDataModels;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

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
        public List<PropertyMapper> PropertyMappers { get; set; }
        public List<SourceDataVariable> SourceDataVariables { get; internal set; }

        public List<PropertyMapper> ClassifierFieldMappers;

        //Add a flag here for the dataset as a whole to check if extra fields need to be added
        //for example if FEATPROP and FEATPROPVAL are needed

        public List<SourceDataFile> SourceFiles;


        public DatasetMapper()
        {
            ObservationMappers = new List<ObservationMapper>();
            PropertyMappers = new List<PropertyMapper>();
            SourceDataVariables = new();
        }


        //write a method here that will retrun ftom the obervation mappers, ther source data files that need
        // to be read, this iwll be used by the top method that will get data from original files and map them to the
        //observation models

        public List<SourceDataFile> InitializeSourceDataFiles(string srcDataPath)
        {
            var sourceFileNames = new HashSet<string>();
            List<SourceDataFile> sourceDataFiles = new();

            sourceFileNames = SourceDataVariables.Select(s => s.SourceFileName).ToHashSet();

            foreach (string filename in sourceFileNames)
            {
                SourceDataFile srcDataFile = new(filename, srcDataPath, SubjectVariableName);
                srcDataFile.SubjectIdVariableName = SubjectVariableName;
                sourceDataFiles.Add(srcDataFile);

                srcDataFile.DataVariables.AddRange(SourceDataVariables.Where(srcVar => srcVar.SourceFileName == filename));
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
            var obsDSdescriptor = new ObservationDatasetDescriptor(DatasetName)
            {
                
                SubjectIdentifierField = new IdentifierField()
                { Name = "SUBJID", Label = "Subject Identifier" },
                StudyIdentifierField = new IdentifierField()
                { Name = "STUDYID", Label = "Study Identifier" },
                FeatureNameField = new DesignationField()
                { Name = "OBSFEAT", Designation = "Name of Feature", Label = "Observed Feature" }
                
            };

            //Feature Property Fields
            if (HasFeatureProperties())
            {
                obsDSdescriptor.FeaturePropertyNameField = new PropertyField()
                { Name = "FEATPROP", Label = "Feature Property Name" };

                obsDSdescriptor.FeaturePropertyValueField = new PropertyValueField()
                { Name = "FEATPROPVAL", Label = "Feature Property Value" };
            }



            //Classifier Fields
            obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
            {
                Name = "DOMAIN",
                Label = "Dataset Domain",
                Order = 1
            });

            obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
            {
                Name = "FEATCAT",
                Label = "Feature Category",
                Order = 2
            });

            obsDSdescriptor.ClassifierFields.Add(new ClassifierFieldType()
            {
                Name = "FEATSCAT",
                Label = "Feature Subcategory",
                Order = 3
            });


            //Property Value Fields
            var dsObsPropMappers = GetPropertyMappers();
            foreach (var pMap in dsObsPropMappers)
            {
                obsDSdescriptor.ObservedPropertyValueFields.Add(new PropertyValueField()
                {
                    Name = pMap.PropertyName,
                    Label = pMap.PropertyName,
                });
                if (pMap.HasUnit())
                {
                    obsDSdescriptor.ObservedPropertyValueFields.Add(new PropertyValueField()
                    {
                        Name = "UNIT[" + pMap.PropertyName + "]",
                        Label = "UNIT[" + pMap.PropertyName + "]",
                    });
                }
                    
            }

            //Observation Property Fields (e.g. time, visits, epoch)
            foreach (var propMapper in PropertyMappers)
            {
                obsDSdescriptor.ObservationPropertyFields.Add(new DatasetField()
                {
                    Name = propMapper.PropertyName,
                    Label = propMapper.PropertyName
                });
            }


            return obsDSdescriptor;
        }

        private List<PropertyMapper> GetPropertyMappers()
        {
            Dictionary<string,PropertyMapper> Properties = new();

            foreach (var propMapper in ObservationMappers
                .SelectMany(obsMapper => obsMapper.PropertyMappers).Where(p=>!p.IsFeatureProperty).OrderBy(pm => pm.Order))
            {
                if (Properties.ContainsKey(propMapper.PropertyName))
                    continue;
                else
                    Properties.Add(propMapper.PropertyName,propMapper);
            }
            return Properties.Values.ToList();
        }

        public bool HasFeatureProperties()
        {
            return ObservationMappers.Exists(o => o.PropertyMappers.Exists(p => p.IsFeatureProperty));
        }


        //READ DATA FROM SRC FILES
        internal PropertyMapper? GetPropertyMapper(string propertyName)
        {
            if (propertyName == null) return null;
            PropertyMapper propMapper = PropertyMappers.Find(p => p.PropertyName.Equals(propertyName));
            return propMapper;
        }

        public string? EvaluatePropertyValueMapper(SubjectSrcDataRow srcDataRow, PropertyMapper? propertyMapper)
        {
            List<string> Values = new();

            if (propertyMapper == null)
                return null;
            
            foreach (var pvMapper in propertyMapper.PropertyValueMappers)
            {
                //var srcDataFile = SourceFiles.Find(f => f.SourceFileName == pvMapper.SourceFileName);
                //var srcDataRow = srcDataFile?.DataRows.Find(r => r.SubjectId == subjectId);

                //Account for data that has multiple timepoints or multiple rows per subjectId;
                //var srcDataRows = srcDataFile?.DataRows.FindAll(r => r.SubjectId == subjectId);
             
                var srcValue = srcDataRow?.DataRecord[pvMapper.SourceVariableId];


                string newVal;
                
                if (SourceDataVariables.Find(v=>v.Identifier == pvMapper.SourceVariableId).IsMultiVal)
                {
                    var vals = srcValue.Split(',');
                    if(vals.Count() > 0)
                    {
                        foreach(var srcval in vals)
                        {
                            newVal = pvMapper.ValueExpression.EvaluateExpression(srcval);
                            if (newVal == null)
                                throw new Exception("Error evaluating value expression for PVmapper: " + pvMapper.ValueExpression);
                            else
                                Values.Add(newVal);
                        }
                    }else
                        throw new Exception("Error evaluating multi value src expression for PVmapper: " + pvMapper.ValueExpression);
                }
                else
                {
                    newVal = pvMapper.ValueExpression.EvaluateExpression(srcValue);

                    if (newVal == null)
                        throw new Exception("Error evaluating value expression for PVmapper: " + pvMapper.ValueExpression);
                    else
                        Values.Add(newVal);
                }

                
            }
            if (Values.Count == 1) return Values[0];
            else
            {
                Values = Values.Distinct().ToList();
                Values.RemoveAll(v => v == "");

                if (Values.Count == 0) return "";

                var arr = "[";
                foreach (var val in Values)
                {
                    arr += val + "|";
                }
                arr = arr.Substring(0, arr.Length - 1) + "]";
                return arr;
            }
        }

        public string? EvaluateFeatureMapper(SubjectSrcDataRow srcDataRow, ObservationMapper obsMapper)
        {
            if (obsMapper == null | obsMapper?.SourceFileName == null)
                return null;

            //var srcDataFile = SourceFiles.Find(f => f.SourceFileName == obsMapper.SourceFileName);
            //var srcDataRow = srcDataFile?.DataRows.Find(r => r.SubjectId == subjectId);
            //var srcValue = srcDataRow?.DataRecord[obsMapper.SourceVariableId];

            var newVal = obsMapper.FeatureNameExpression.EvaluateExpression(srcDataRow.DataRecord);

            if (newVal == null)
                throw new Exception("Error evaluating value expression for PVmapper: " + obsMapper.FeatureNameExpression);
            else
                return newVal;
        }

    }
}

