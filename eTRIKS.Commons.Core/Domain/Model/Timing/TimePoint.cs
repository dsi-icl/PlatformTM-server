using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.Timing
{
    public abstract class TimePoint : Identifiable<int>
    {
        public string Name { get; set; }
        public int? Number { get; set; }
    }
}
