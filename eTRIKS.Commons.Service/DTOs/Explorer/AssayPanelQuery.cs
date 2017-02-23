using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class AssayPanelQuery
    {
        public int AssayId { get; set; }
        //public List<ObservationRequestDTO> FeatureQuery { get; set; }
        public List<ObservationRequestDTO> SampleQuery { get; set; }
        //public List<ObservationRequestDTO> ObservationMeasureQuery { get; set; }

        public AssayPanelQuery()
        {
            SampleQuery = new List<ObservationRequestDTO>();
        }
    }
}
