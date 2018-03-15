using PlatformTM.Core.Domain.Model.DesignElements;
using PlatformTM.Core.Domain.Model.Timing;

namespace PlatformTM.Core.JoinEntities
{
    public class VisitTimepoints
    {
        public Visit Visit { get; set; }
        public int VisitId { get; set; }
        public TimePoint Timepoint { get; set; }
    }
}
