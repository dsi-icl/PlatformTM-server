using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKSService.Models.ControlledTerminology;


namespace eTRIKSService.Models
{
    public class DomainVariable
    {
        public string OID { get; set; }
        public string domainVariableName { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public CVterm variableType { get; set; }
        public CVterm role { get; set; }
        public DomainDataset parentDomain { get; set; }
    }
}