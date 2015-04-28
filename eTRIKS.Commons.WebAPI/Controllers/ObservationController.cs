using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Core.Domain.Model;
using System.Threading.Tasks;

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
        public Task<List<SubjectObservation>> LoadObservations(string studyId){
           // _observationService.loadObservations(studyId);
           return  _observationService.test();
           // return _observationService.loadTest();
        }
    }
}
