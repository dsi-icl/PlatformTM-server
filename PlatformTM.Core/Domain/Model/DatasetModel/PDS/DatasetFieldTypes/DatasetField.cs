using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes
{
    public class DatasetField : Identifiable<int>
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ContentDataType { get; set; }
        public DatasetFieldType FieldType{ get; set; }
        //public string DataElementId { get; set; }

        public DatasetField()
        {
        }
    }

    public class IdentifierField : DatasetField
    {
        public string MasterEntity { get; set; }
        public IdentifierField()
        {
            FieldType = DatasetFieldType.IdentifierFieldType;
        } 

    }
    public class DesignationField : DatasetField
    {
        public string MasterEntity { get; set; }
        public string Designation { get; set; }
        public DesignationField()
        {
            FieldType = DatasetFieldType.DesignationFieldType;
        }
    }
    public class ClassifierFieldType : DatasetField
    {
        public int Order { get; set; }
        public ClassifierFieldType()
        {
            FieldType = DatasetFieldType.ClassifierFieldType;
        }

    }
    public class PropertyField : DatasetField
    {
        public string ObjectClass { get; set; }
        public PropertyField()
        {
            FieldType = DatasetFieldType.PropertyFieldType;
        }
    }
    public class PropertyValueField : DatasetField
    { 
        public string PropertyName { get; set; }
        public PropertyValueField()
        {
            FieldType = DatasetFieldType.PropertyValueFieldType;
        }
    }
    public class QualifiedPropertyValueField : DatasetField
    {
        public string PropertyTerm { get; set; }
        public string QualifierTerm { get; set; }
    }

    public class TimeSeriesField : DatasetField
    {
        public string TimeSeriesProperty { get; set; }
        public TimeSeriesField()
        {
            FieldType = DatasetFieldType.TimeSeriesFieldType;
        }
    }

  


    //public abstract class FieldConstruct
    //{
    //    public List<Field> Fields { get; set; }

    //}

    //public class Field
    //{

    //}

    //public class CodedTextFG : DatasetField
    //{
    //    public DatasetField Value { get; set; }
    //    public DatasetField Code { get; set; }
    //    public DatasetField Terminology { get; set; }
    //}

    //public class ObsrvationDataset
    //{
    //    public CodedTextFG Topic { get; set; }
    //    public List<DatasetField> ObservedProprties { get; set; }
    //    public void AddObservedProperty()
    //    {
    //        var f = new CodedTextFG();
    //        f.Code = new DatasetField();
    //        f.Value = new DatasetField();
    //        f.Terminology = new DatasetField();
    //        ObservedProprties.Add(f);

    //    }
    //}


}
