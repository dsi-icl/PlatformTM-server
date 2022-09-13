using System.Collections.Generic;

namespace PlatformTM.Models.DTOs.Explorer
{
    public class AssayPanelQueryDTO
    {
        public int AssayId { get; set; }
        //public List<ObservationRequestDTO> FeatureQuery { get; set; }
        public List<ObservationRequestDTO> SampleQuery { get; set; }
        //public List<ObservationRequestDTO> ObservationMeasureQuery { get; set; }

        public AssayPanelQueryDTO()
        {
            SampleQuery = new List<ObservationRequestDTO>();
        }
    }
}
