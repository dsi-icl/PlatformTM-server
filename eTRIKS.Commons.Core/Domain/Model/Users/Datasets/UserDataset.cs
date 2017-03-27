
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Datasets
{
    public class UserDataset : Identifiable<Guid>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string OwnerId { get; set; }
        public int ProjectId { get; set; }
        public List<DatasetField> Fields { get; set; }
        public List<DataFilter> Filters { get; set; }
        public string LastModified { get; set; }
        public string Type { get; set; }
        public Guid QueryId { get; set; }
        public int SubjectCount { get; set; }
        public int SampleCount { get; set; }
        // add the fields
        public string ExportFileURI { get; set; }
        public bool FileIsReady { get; set; }




        public UserDataset()
        {
            Fields = new List<DatasetField>();
            Filters = new List<DataFilter>();
        }
    }


    public class DataField
    {
        public string FieldName { get; set; }   //Header for the data field
        public string Entity { get; set; }      //The model entity to be queried
        public int EntityId { get; set; }       //
        public string Property { get; set; }    //The property of the model to be added as the data field
        public int PropertyId { get; set; }  //The id of the property if normalized as objects (Only applicable in case of Characteristics and Observations)
        //public string ValuesProperty { get; set; } //Only applicable in case of observations and characteristics
        public string DataType { get; set; }
        public bool IsFiltered { get; set; }
    }

    [KnownType(typeof(DataFilterRange))]
    [KnownType(typeof(DataFilterExact))]
    public class DataFilter
    {
        public DataField DataField { get; set; }
    }
    public class DataFilterRange : DataFilter
    {
        //public Range Range { get; set; }
        public double Upperbound { get; set; }
        public double Lowerbound { get; set; }
    }
    public class DataFilterExact : DataFilter
    {
        public List<string> Values { get; set; }

        public DataFilterExact()
        {
            Values = new List<string>();
        }
    }
}
