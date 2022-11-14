using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;

namespace PlatformTM.Core.JoinEntities
{
    public class ObservationQualifier
    {
        public int ObservationId{ get; set; }
        //public Observation Observation{ get; set; }
        public int QualifierId { get; set; }
        public VariableDefinition Qualifier { get; set; }
    }
}
