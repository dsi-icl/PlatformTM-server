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
        public List<ClassifierFieldType> ClassifierFields { get; set; }

        public DatasetField ObservationProperty { get; set; }

        public TimeSeriesField TimeSeriesField { get; set; }

        public List<PropertyValueField> PropertyValueFields { get; set; }

        public List<DatasetField> ObservedPropertyFields { get; set; }
        public List<QualifiedPropertyValueField> QualifiedPropertyValueFields { get; set; }




    }
}
