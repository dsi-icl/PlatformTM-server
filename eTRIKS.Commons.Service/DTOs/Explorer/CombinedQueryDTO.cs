using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class CombinedQueryDTO
    {
      public string Name {get;set;}
      public List<ObservationRequestDTO> ObsRequests { get; set; }
     
//********************************************************************************************************************************************************
      public List<AssayPanelQueryDTO> AssayPanels { get; set; }

      public CombinedQueryDTO()
      {
            ObsRequests = new List<ObservationRequestDTO>();
            AssayPanels = new List<AssayPanelQueryDTO>();
       }
//********************************************************************************************************************************************************

    }
}
