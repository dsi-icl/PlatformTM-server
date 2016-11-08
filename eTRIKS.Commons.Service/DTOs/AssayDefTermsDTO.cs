using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System.Collections.Generic;

namespace eTRIKS.Commons.Service.DTOs
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
