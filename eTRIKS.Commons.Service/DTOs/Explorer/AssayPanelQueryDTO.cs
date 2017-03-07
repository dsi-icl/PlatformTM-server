using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs.Explorer
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
