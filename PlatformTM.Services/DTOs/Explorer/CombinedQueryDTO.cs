using System.Collections.Generic;

namespace PlatformTM.Models.DTOs.Explorer
{
    public class CombinedQueryDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ObservationRequestDTO> ObsRequests { get; set; }
        public List<ObservationRequestDTO> SubjCharRequests { get; set; }
        public Dictionary<int,AssayPanelDTO> AssayPanelRequests { get; set; }
        public bool IsSavedByUser { get; set; }

        public string UserId { get; set; }
        public int ProjectId { get; set; }

        public CombinedQueryDTO()
        {
            SubjCharRequests = new List<ObservationRequestDTO>();
            ObsRequests = new List<ObservationRequestDTO>();
            AssayPanelRequests = new Dictionary<int, AssayPanelDTO>();
        }
    }
}
