using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKSService.Models.ControlledTerminology;

namespace eTRIKSService.Models
{
    public class DerivedVarMethod
    {
        public string methodId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string formalExpression { get; set; }
        public CVterm type { get; set; }
        public string derivedVariableId { get; set; }

        public virtual VariableDefinition derivedVariable { get; set; }

        public virtual ICollection<VariableDefinition> SourceVariables {get; set;}
    }
}