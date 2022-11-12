using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.DesignElements
{
    public class Cohort : Identifiable<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public List<Study> Studies { get; set; }

        public List<HumanSubject> Subjects { get; set; }
        

        public Cohort()
        {
            Studies = new List<Study>();
            Subjects = new List<HumanSubject>();
        }
    }
}
