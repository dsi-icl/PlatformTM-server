using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class CombinedQueryDTO
    {
      public string Name {get;set;}
      public List<ObservationRequestDTO> ObsRequest { get; set; }

    }
}
