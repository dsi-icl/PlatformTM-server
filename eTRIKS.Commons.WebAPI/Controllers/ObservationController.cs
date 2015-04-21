using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class ObservationController : ApiController
    {
        private ObservationService _observationService;

        public ObservationController(ObservationService observationService)
        {
            _observationService = observationService;
        }

        [HttpGet]
        public void LoadObservations(string studyId){
            _observationService.loadObservations(studyId);
        }
    }
}
