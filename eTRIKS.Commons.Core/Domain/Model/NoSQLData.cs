using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class MongoDataCollection
    {
        public List<MongoDocument> documents = new List<MongoDocument>();
    }

    public class MongoDocument : Identifiable<Guid>
    {
        public List<MongoField> fields = new List<MongoField>();
    }

    public class MongoField
    {
        public string Name { get; set; }
        public string value { get; set; }
    }



    public class SubjObservationTemp
    {
        public string subjId { get; set; }
        public string test { get; set; }
        public string value { get; set; }
        public int visitNo { get; set; }
        public string timepoint { get; set; }
        public int day { get; set; }
    }

    public class NoSQLRecordForUpdate
    {
        public List<MongoField> CurrentRecord = new List<MongoField>();
        public List<MongoField> NewRecord = new List<MongoField>();
    }



}