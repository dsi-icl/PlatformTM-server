using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model.DatasetDescriptorTypes
{
    public class ObservationDatasetDescriptor : DatasetDescriptor
    {



        public IdentifierField StudyIdentifierField { get; set; } 
        public IdentifierField SubjectIdentifierField { get; set; }
        public IdentifierField SampleIdentifierField { get; set; }

        public DesignationField FeatureNameField { get; set; }
        public List<ClassifierFieldType> ClassifierFields { get; set; } = new List<ClassifierFieldType>();

        


        public List<PropertyValueField> PropertyValueFields { get; set; } = new List<PropertyValueField>();
        public List<QualifiedPropertyValueField> QualifiedPropertyValueFields { get; set; }

        public PropertyField FeaturePropertyNameField { get; set; }
        public PropertyValueField FeaturePropertyValueField { get; set; }

        public List<DatasetField> ObservedPropertyFields { get; set; }

        public List<DatasetField> ObservationPropertyFields { get; set; } = new List<DatasetField>();


        public TimeSeriesField TimeSeriesField { get; set; }


        public ObservationDatasetDescriptor()
        {

        }

        public ClassifierFieldType GetClassifierField(int order)
        {

            return ClassifierFields.Find(f => f.Order == order);
        }

    }
}
