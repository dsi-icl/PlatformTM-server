using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.DesignElements
{
    public class Arm : Identifiable<string>
    {
        //CONSIDER PUTTING BACK WHEN EF SUPPORTS M-2-M RELATIONSHIPS
        //public List<Study> Studies { get; set; }
        public List<StudyArm> Studies { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public Arm()
        {
            Studies = new List<StudyArm>();
        }
    }
}
