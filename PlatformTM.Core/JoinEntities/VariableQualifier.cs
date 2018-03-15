using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.JoinEntities
{
    public class VariableQualifier
    {
        public int VariableId {get; set; }
        public VariableDefinition Variable { get; set; }

        public int QualifierId { get; set; }
        public VariableDefinition Qualifier { get; set; }
    }
}
