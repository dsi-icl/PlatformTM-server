using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;

namespace PlatformTM.API.Controllers
{
    [Route("terms")]
    public class TermController : Controller
    {
        private CVtermService _cvtermService;

        public TermController(CVtermService cvTermService)
        {
            _cvtermService = cvTermService;
        }

        [HttpGet("assay/measurementTypes")]
        public List<AssayDefTermsDTO> GetAssayMeasurementTypes()
        {
            return _cvtermService.GetAssayDefTerms();
        }
    }
}
