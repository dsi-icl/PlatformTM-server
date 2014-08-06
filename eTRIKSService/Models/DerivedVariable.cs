using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKSService.Models.ControlledTerminology;

namespace eTRIKSService.Models
{
    public class DerivedVariable : DomainVariable
    {
        public string methodDescription { get; set; }
        public string formatExpression { get; set; }
        public CVterm type { get; set; }
        public List<DomainVariable> sourceVariables;
    }
}