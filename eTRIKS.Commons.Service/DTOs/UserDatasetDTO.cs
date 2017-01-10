using System;
using System.Collections.Generic;
using System.Dynamic;

namespace eTRIKS.Commons.Service.DTOs
{
    public class UserDatasetDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string OwnerId { get; set; }
        public int ProjectId { get; set; }
        public string ProjectAcc { get; set; }
        public string Type { get; set; }
        public string LastModified { get; set; }
        public List<DataFieldDTO> Fields { get; set; }
        public List<DataFilterDTO> Filters { get; set; }
        //public List<ComputedDataField> ComputedFields { get; set; }

        public  UserDatasetDTO()
        {
            Fields = new List<DataFieldDTO>();
            Filters = new List<DataFilterDTO>();
        }


    }



    public class DataFilterDTO
    {
        public DataFieldDTO Field { get; set; }
        public bool IsNumeric { get; set; }
        public double Min { get; set; } //the minimum value available for this field set from db
        public double Max { get; set; } //the maximum value available for this field set from db
        public double From { get; set; } //the from value selected by user
        public double To { get; set; } //the to value selected by user
        public List<string> FilterValues { get; set; } //the set of values selected by the user
        public List<string> ValueSet { get; set; } // the set of all available values for this field
        public string Unit { get; set; }
    }

    public class DataFieldDTO
    {
        public string FieldName { get; set; }   //Header for the data field
        public string Entity { get; set; }      //The model entity to be queried
        public int EntityId { get; set; }       //
        public string Property { get; set; }    //The property of the model to be added as the data field
        public int PropertyId { get; set; }  //The id of the property if normalized as objects (Only applicable in case of Characteristics and Observations)
        //public string ValuesProperty { get; set; } //Only applicable in case of observations and characteristics
        private string _columnHeader;
        public string ColumnHeader
        {
            get{ return Entity + (Property != null ? "[" + Property + "]" : ""); } 
            set { _columnHeader = value; }
        }

        public string DataType { get; set; }
        public bool IsFiltered { get; set; }
    }

    //public class ComputedDataField
    //{
    //    public string FieldName { get; set; }
    //    public string Description { get; set; }
    //    public string Function { get; set; }
    //    public List<ExpressionElement> ExpressionList { get; set; }

    //    public class ExpressionElement
    //    {
    //        public string Name { get; set; }
    //        public string Type { get; set; }
    //        public string SubType { get; set; }
    //        public bool IsUserInput { get; set; }
    //    } 
    //}


}
