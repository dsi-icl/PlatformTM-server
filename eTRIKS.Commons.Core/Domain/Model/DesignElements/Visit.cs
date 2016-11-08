using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.Domain.Model.DesignElements
{
    public class Visit : Identifiable<int>
    {
        public string Name { get; set; }
        public int? Number { get; set; }
        public ICollection<TimePoint> TimePoints { get; set; }
        public RelativeTimePoint StudyDay { get; set; }
        public int StudyDayId { get; set; }
        public Study Study { get; set; }
        public int StudyId { get; set; }
    }
}
