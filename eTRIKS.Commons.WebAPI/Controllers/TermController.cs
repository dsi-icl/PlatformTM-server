using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/terms")]
    public class TermController : ApiController
    {
        private CVtermService _cvtermService;

        public TermController(CVtermService cvTermService)
        {
            _cvtermService = cvTermService;
        }


        [HttpGet]
        [Route("assay/measurementTypes")]
        public List<CVterm> GetAssayMeasurementTypes()
        {
            
            return _cvtermService.GetAssayDefTerms(); 
        }
    }
}
