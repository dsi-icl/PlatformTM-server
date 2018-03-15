namespace PlatformTM.Services.DTOs
{
    public class AssayDataDTO
    {
        public string FeatureName { get; set; }
        public string SubjectOfObservationName { get; set; }
        public double Value { get; set; }
        public int SubjectOfObservationId { get; internal set; }
    }
}

