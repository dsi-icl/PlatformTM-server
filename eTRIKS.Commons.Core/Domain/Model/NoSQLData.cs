using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class NoSQLData
    {
        public List<Record> records = new List<Record>();
    }

    public class Record
    {
        public List<RecordItem> recordItems = new List<RecordItem>();
    }

    public class RecordItem
    {
        public string fieldName { get; set; }
        public string value { get; set; }
    }

}