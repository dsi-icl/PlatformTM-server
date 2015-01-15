using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class VariableReference
    { 
        public int? OrderNumber { get; set; }
        public bool? IsRequired { get; set; }
        public int? KeySequence { get; set; }

        public VariableDefinition VariableDefinition { get; set; }
        public int VariableDefinitionId { get; set; }

        public Dataset Dataset { get; set; }
        public int DatasetId { get; set; }
    }
}