using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.ControlledTerminology;

namespace PlatformTM.Models.DTOs
{
    public class AssayDefTermsDTO
    {
        public CVterm AssayTypeTerm { get; set; }
        public List<CVterm> AssayPlatTerms { get; set; }
        public List<CVterm> AssayTechTerms { get; set; }

        public AssayDefTermsDTO()
        {
            AssayPlatTerms = new List<CVterm>();
            AssayTechTerms = new List<CVterm>();
        }
    }
}
