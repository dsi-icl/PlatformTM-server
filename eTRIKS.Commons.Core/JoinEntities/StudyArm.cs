using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;

namespace eTRIKS.Commons.Core.JoinEntities
{
    public class StudyArm
    {
        public string ArmId { get; set; }
        public Arm Arm { get; set; }

        public int StudyId { get; set; }
        public Study Study { get; set; }
    }
}
