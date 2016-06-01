using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
