using System;
using System.Collections.Generic;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class DatasetField
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string ContentDataType { get; set; }
        public string DataElementId { get; set; }

        public DatasetField()
        {
        }
    }

    public class IdentifierField : DatasetField
    {
        public string MasterEntity { get; set; }

    }
    public class DesignationField : DatasetField
    {
        public string Designation { get; set; }
    }
    public class ClassifierFieldType : DatasetField
    {
        public int Order { get; set; }
    }
    public class PropertyField : DatasetField
    {
        public string ObjectClass { get; set; }
    }
    public class PropertyValueField : DatasetField
    {
        public string FeatureName { get; set; }
        public string PropertyName { get; set; }
    }
    public class QualifiedPropertyValueField : DatasetField
    {
        public string PropertyTerm { get; set; }
        public string QualifierTerm { get; set; }
    }


    public class SubjectIdentifierField: IdentifierField
    {

    }




    public class TimeSeriesField : DatasetField
    {
        public string TimeSeriesProperty { get; set; }
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
