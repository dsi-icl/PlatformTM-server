using System.Collections.Generic;

namespace PlatformTM.Models.DTOs.Explorer
{
    public class AssayPanelDTO
    {
        public int AssayId { get; set; }
        //public List<ObservationRequestDTO> FeatureQuery { get; set; }
        public List<ObservationRequestDTO> SampleQuery { get; set; }
        public string AssayName { get; set; }
        public bool IsRequested { get; set; }

        //public List<ObservationRequestDTO> ObservationMeasureQuery { get; set; }

        public AssayPanelDTO()
        {
            SampleQuery = new List<ObservationRequestDTO>();
        }
    }
}
