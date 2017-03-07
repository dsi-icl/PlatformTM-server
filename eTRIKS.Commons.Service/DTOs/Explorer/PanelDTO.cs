using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class PanelDTO
    {
        public int AssayId { get; set; }
        //public List<ObservationRequestDTO> FeatureQuery { get; set; }
        public List<ObservationRequestDTO> SampleQuery { get; set; }
        public string AssayName { get; set; }

        //public List<ObservationRequestDTO> ObservationMeasureQuery { get; set; }

        public PanelDTO()
        {
            SampleQuery = new List<ObservationRequestDTO>();
        }
    }
}
