using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class VariableReference : Identifiable<string>
    { 
        public int? OrderNumber { get; set; }
        public bool? IsRequired { get; set; }
        public int? KeySequence { get; set; }

        public VariableDefinition VariableDefinition { get; set; }
        public string VariableDefinitionId { get; set; }

        public Dataset Dataset { get; set; }
        public string DatasetId { get; set; }
    }
}