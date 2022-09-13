using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.ObservationModel
{
    public class ObservedFeature : Identifiable<int>
    {

        public string FeatureName { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public ObservedFeature()
        {
        }
    }
}

