using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class SuggestedMap
    {
        public string SourceHeader { get; set; }
        public bool IsFinalised { get; set; }
        public string MappedVariable { get; set; } 
        public string MappedVariableSynonym { get; set; }
        public string MappedVariableDefinition { get; set; }
        public string MappedDomain { get; set; }

        public bool IsIdentifier { get; set; }
        public bool IsTopic { get; set; }
        public bool IsQualifier { get; set; }
        public bool IsUnitRequired { get; set; }
        public bool IsUnit { get; set; }

    }
}
