using System.Collections.Generic;

namespace PlatformTM.Models.DTOs.Explorer
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
