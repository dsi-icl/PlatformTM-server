using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DesignElements;

namespace PlatformTM.Core.JoinEntities
{
    public class StudyArm
    {
        public string ArmId { get; set; }
        public Arm Arm { get; set; }

        public int StudyId { get; set; }
        public Study Study { get; set; }
    }
}
