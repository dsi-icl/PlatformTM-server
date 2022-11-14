using System;
namespace PlatformTM.Services.DatasetExplorer
{
    public class ObservationRequestDTO
    {

        public string ObsPhenoName { get; set; }
        public int ObsPhenoId { get; set; }
        public string DatasetId { get; set; }
        public string obsFeature { get; set; }
        public string obsProperty { get; set; }
        public int ObsFeatureId { get; set; }
        public int ObsPropertyId { get; set; }

        public ObservationRequestDTO()
        {
            
        
        }
    }
}

