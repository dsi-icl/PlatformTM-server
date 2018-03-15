using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Services.Services
{

    public class CVtermService
    {
        private readonly IRepository<CVterm, string> _CVrepository;

        public CVtermService(IServiceUoW uoW)
        {
            _CVrepository = uoW.GetRepository<CVterm, string>();
        }

        public List<AssayDefTermsDTO> GetAssayDefTerms()
        {
           List<AssayDefTermsDTO> terms = new List<AssayDefTermsDTO>();
            //TODO: TEMP ... SHOULD BE REPLACED BY A CALL TO THE ONTOLOGY LOOKUP SERVICE
           var assayMeasurementTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYMT")).ToList();
            var assayPlatformTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYTP")).ToList();
            var assayPlatTechTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYTT")).ToList();

            int i = 0;
            foreach(var term in assayMeasurementTypes)
            {
                var assayCVterms = new AssayDefTermsDTO();
                assayCVterms.AssayTypeTerm = term;
                assayCVterms.AssayPlatTerms.Add(assayPlatformTypes[i]);
                assayCVterms.AssayTechTerms.Add(assayPlatTechTypes[i]);
                terms.Add(assayCVterms);
                i++;
            }

            return terms;
        }
    }
}
