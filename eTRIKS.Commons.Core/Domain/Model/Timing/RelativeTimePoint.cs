namespace eTRIKS.Commons.Core.Domain.Model.Timing
{
    public class RelativeTimePoint : TimePoint
    {
        public TimePoint ReferenceTimePoint { get; set; }
        public int? ReferenceTimePointId { get; set; }
    }
}
