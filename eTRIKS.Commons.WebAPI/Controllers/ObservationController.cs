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

        //[HttpGet]
        //[Route("api/ObservationTest")]
        //public async Task testSerializer()
        //{
        //    await _observationService.testSerializer();
        //}

        //[HttpGet]
        //public async Task LoadObservations(string studyId){
        //    await _observationService.loadObservations(studyId);
        //   //return  _observationService.test();
        //   // return _observationService.loadTest();
        //}

        //[HttpGet]
        //[Route("api/projects/{projectId}/observations")]
        //public void getObsInventory(string projectId)
        //{
        //    _observationService.getObservationInventory(projectId);
        //}
    }
}
