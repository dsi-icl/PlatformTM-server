using System.Collections.Generic;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/terms")]
    public class TermController : Controller
    {
        private CVtermService _cvtermService;

        public TermController(CVtermService cvTermService)
        {
            _cvtermService = cvTermService;
        }


        [HttpGet]
        [Route("assay/measurementTypes")]
        public List<AssayDefTermsDTO> GetAssayMeasurementTypes()
        {

            return _cvtermService.GetAssayDefTerms();
        }
    }
}
