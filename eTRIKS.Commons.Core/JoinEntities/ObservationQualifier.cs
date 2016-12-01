
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;

namespace eTRIKS.Commons.Core.JoinEntities
{
    public class ObservationQualifier
    {
        public int ObservationId{ get; set; }
        public Observation Observation{ get; set; }
        public int QualifierId { get; set; }
        public VariableDefinition Qualifier { get; set; }
    }
}
