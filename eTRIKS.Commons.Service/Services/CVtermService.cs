using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    
    public class CVtermService
    {
        private readonly IRepository<CVterm, string> _CVrepository;

        public CVtermService(IServiceUoW uoW)
        {
            _CVrepository = uoW.GetRepository<CVterm, string>();
        }

        public List<CVterm> GetAssayDefTerms()
        {
           var assayMeasurementTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYMT")).ToList();
            var assayPlatformTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYPT")).ToList();
            var assayPlatTechTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYTT")).ToList();

           
            foreach(var term in assayMeasurementTypes)
            {
                var assayCVterms = new AssayCVtermsDTO();
                assayCVterms.AssayMeasurementType = term;


            }

            return assayMeasurementTypes;
        }
    }
}
