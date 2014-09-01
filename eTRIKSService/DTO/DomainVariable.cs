using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.DTO
{
    public class DomainVariable
    {
        public string OID { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public string datasetId { get; set; }
        public string variableType { get; set; }
        public string role { get; set; }


    }
}