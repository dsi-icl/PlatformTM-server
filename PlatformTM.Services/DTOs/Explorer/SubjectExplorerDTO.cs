using System.Collections.Generic;

namespace PlatformTM.Services.DTOs.Explorer
{
    public class SubjectExplorerDTO
    {
        public List<ObservationRequestDTO> Characteristics { get; set; }
        public List<ObservationRequestDTO> Timings { get; set; }
        public List<ObservationRequestDTO> DesignElements { get; set; }

        public SubjectExplorerDTO()
        {
            Characteristics = new List<ObservationRequestDTO>();
            Timings = new List<ObservationRequestDTO>();
            DesignElements = new List<ObservationRequestDTO>();
        }
    }
}
