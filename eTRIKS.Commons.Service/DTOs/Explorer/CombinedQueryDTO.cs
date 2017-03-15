using System;
using System.Collections.Generic;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class CombinedQueryDTO
    {
        public string Name { get; set; }
        public List<ObservationRequestDTO> ObsRequests { get; set; }
        public List<ObservationRequestDTO> SubjCharRequests { get; set; }
        public Dictionary<int,AssayPanelDTO> AssayPanelRequests { get; set; }
        //public List<AssayPanelDTO> AssayPanelRequests { get; set; }

        public string UserId { get; set; }
        public CombinedQueryDTO()
        {
            SubjCharRequests = new List<ObservationRequestDTO>();
            ObsRequests = new List<ObservationRequestDTO>();
            AssayPanelRequests = new Dictionary<int, AssayPanelDTO>();
        }
    }
}
