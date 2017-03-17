using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs.Explorer
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
