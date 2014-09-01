using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class VariableReference
    {
        public string variableId { get; set; }
        public string activityId { get; set; }
        public Nullable<int> orderNo { get; set; }
        public Nullable<bool> mandatory { get; set; }
        public Nullable<int> keySequence { get; set; }

        public virtual VariableDefinition Variable_Def { get; set; }
    }
}