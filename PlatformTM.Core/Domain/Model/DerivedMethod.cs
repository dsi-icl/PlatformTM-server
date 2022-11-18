using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

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

        public virtual DatasetField DerivedField { get; set; }
        public CVterm DerivedValueType { get; set; }
        public virtual ICollection<DatasetField> SourceFields {get; set;}
    }
}