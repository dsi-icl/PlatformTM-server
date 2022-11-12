using System;
using System.Collections.Generic;
using System.Data;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes
{
    public class ObservationDatasetDescriptor : DatasetDescriptor
    {

        public IdentifierField StudyIdentifierField { get; set; } 
        public IdentifierField SubjectIdentifierField { get; set; }
        public IdentifierField SampleIdentifierField { get; set; }

        public DesignationField FeatureNameField { get; set; }
        public List<ClassifierFieldType> ClassifierFields { get; set; } = new List<ClassifierFieldType>();

        

        //e.g. OCCURRENCE, DIAGNOSIS ...etc
        public List<PropertyValueField> ObservedPropertyValueFields { get; set; } = new List<PropertyValueField>();
        public List<QualifiedPropertyValueField> QualifiedPropertyValueFields { get; set; }

        //e.g. Breathing NOISE - SOUND i.e. FA an event or treatment
        public PropertyField FeaturePropertyNameField { get; set; }
        public PropertyValueField FeaturePropertyValueField { get; set; }


        public List<DatasetField> ObservationPropertyFields { get; set; } = new List<DatasetField>();

        public TimeSeriesField TimeSeriesField { get; set; }


        public ObservationDatasetDescriptor()
        {

        }

        public ObservationDatasetDescriptor(string DatasetName)
        {
            Title = DatasetName;
            SubjectIdentifierField = new IdentifierField()
            { Name = "SUBJID", Label = "Subject Identifier" };
            StudyIdentifierField = new IdentifierField()
            { Name = "STUDYID", Label = "Study Identifier" };
            FeatureNameField = new DesignationField()
            { Name = "OBSFEAT", Designation = "Name of Feature", Label = "Observed Feature" } ;
            DatasetType = DatasetType.ObservationDatasetDescriptor;
        }

        public ClassifierFieldType GetClassifierField(int order)
        {

            return ClassifierFields.Find(f => f.Order == order);
        }

        public override List<DatasetField> GetDatasetFields()
        {

            var allFields = new List<DatasetField>();

            allFields.Add(StudyIdentifierField);
            allFields.Add(SubjectIdentifierField);
            

            allFields.AddRange(ClassifierFields);
            allFields.Add(FeatureNameField);

            if (FeaturePropertyNameField != null) allFields.Add(FeaturePropertyNameField);
            if (FeaturePropertyValueField != null) allFields.Add(FeaturePropertyValueField);

            allFields.AddRange(ObservedPropertyValueFields);
            allFields.AddRange(ObservationPropertyFields);
            
            return allFields;
        }

        public DataTable GetDatasetAsDatatable()
        {
            var descDT = new DataTable
            {
                TableName = Title
            };



            descDT.Columns.Add("FIELD_NAME");
            descDT.Columns.Add("FIELD_LABEL");
            descDT.Columns.Add("FIELD_DESCRIPTION");
            descDT.Columns.Add("FIELD_TYPE");
            descDT.Columns.Add("FIELD_NAME_TERMID");
            descDT.Columns.Add("FIELD_NAME_TERMREF");

            descDT.Rows.Add(StudyIdentifierField.Name, StudyIdentifierField.Label, "", "IdentifierField", "", "");
            descDT.Rows.Add(SubjectIdentifierField.Name, SubjectIdentifierField.Label, "", "IdentifierField", "", "");

            ClassifierFields.Sort((a, b) => (a.Order.CompareTo(b.Order)));
            foreach (var classifierField in ClassifierFields)
            {
                descDT.Rows.Add(classifierField.Name, classifierField.Label, "", "ClassifierFieldType", "", "");
            }

            descDT.Rows.Add(FeatureNameField.Name, FeatureNameField.Label, "", "DesignationField", "", "");

            foreach (var propertyValueField in ObservedPropertyValueFields)
            {
                descDT.Rows.Add(propertyValueField.Name, "", "", "PropertyValueField", "", "");
            }

            foreach (var timeField in ObservationPropertyFields)
            {
                descDT.Rows.Add(timeField.Name, "", "", "PropertyValueField", "", "");
            }

            return descDT;
        }
    }
}
