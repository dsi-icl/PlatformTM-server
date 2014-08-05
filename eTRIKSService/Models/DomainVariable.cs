using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class DomainVariable
    {
        public string OID { get; set; }
        public string domainVariableName { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public string variableType { get; set; }
        public string role { get; set; }
    }
}