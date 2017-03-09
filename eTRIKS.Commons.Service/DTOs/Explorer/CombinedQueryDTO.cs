using System.Collections.Generic;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class CombinedQueryDTO
    {
      public string Name {get;set;}
      public List<ObservationRequestDTO> ObsRequests { get; set; }

      public List<PanelDTO> AssayPanels { get; set; }

    }
}
