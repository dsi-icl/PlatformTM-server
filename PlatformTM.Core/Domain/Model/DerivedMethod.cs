using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.Domain.Model
{
    public class DerivedMethod : Identifiable<string>
    {
        //public string MethodId { get; set; }
        public string MethodName { get; set; }
        public string MethodDescription { get; set; }
        public string FormalExpression { get; set; }
        public string DerivedVariableId { get; set; }
        public string DerivedValueTypeId { get; set; }

        public virtual VariableDefinition DerivedVariable { get; set; }
        public CVterm DerivedValueType { get; set; }
        public virtual ICollection<VariableDefinition> SourceVariables {get; set;}
    }
}