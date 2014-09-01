using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class VariableReference
    {
        public Nullable<int> OrderNumber { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public Nullable<int> KeySequence { get; set; }

        public VariableDefinition Variable { get; set; }
        public string VariableId { get; set; }

        public Dataset Dataset { get; set; }
        public string DatasetId { get; set; }
    }
}