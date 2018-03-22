using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class AssayBrowserDTO
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public string Technology { get; set; }
        public string Type { get; set; }
        public string Design { get; set; }
        public string Name { get; set; }

        public List<ObservationRequestDTO> SampleCharacteristics { get; set; }
        public List<ObservationRequestDTO> FeatureAnnotations { get; set; }
        public AssayPanelDTO AssayPanel { get; set; }
        
    }
}
