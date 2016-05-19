using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.Services
{
    public class CVtermService
    {
        private readonly IRepository<CVterm, string> _CVrepository;

        public CVtermService(IServiceUoW uoW)
        {
            _CVrepository = uoW.GetRepository<CVterm, string>();
        }

        public void GetAssayDefTerms()
        {
           var assayMeasurementTypes = _CVrepository.FindAll(cv => cv.DictionaryId.Equals("CL-ASYMT")).ToList();

        }
    }
}
