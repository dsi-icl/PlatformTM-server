using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.Services;
using System.Collections;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    //[RoutePrefix("api/studies")]
    public class DataController : ApiController
    {
        private DataService _dataService;

        public DataController(DataService dataService)
        {
            _dataService = dataService;
        }

        //[HttpPost]
        //[Route("api/studies/{studyId}/data/clinical/{domainCode}/observations")]
        //public async Task<List<Hashtable>> getObservations(string studyId, string domainCode, [FromBody] List<string> observations)
        //{
        //    //return await _dataService.getObservationsData(studyId, new List<int>());
        //    return await _dataService.getObservationsData(studyId, domainCode, observations);
        //}

        //[HttpGet]
        //[Route("api/TEST/studies/{studyId}/data/clinical/{domainCode}/observations")]
        //public async Task<List<Hashtable>> getObservations(string studyId, string domainCode)
        //{
        //    //return await _dataService.getObservationsData(studyId, new List<int>());
        //    List<string> observations = new List<string> { "BMI", "TEMP" };
        //    return await _dataService.getObservationsData(studyId, domainCode, observations);
        //}
        [HttpGet]
        [Route("api/projects/{projectId}/subjects/characteristics")]
        public List<SubjCharDTO> getSubjectCharacteristics(string projectId)
        {
            return _dataService.getSubjectCharacteristics(projectId);
        }

        [HttpPost]
        [Route("api/studies/{studyId}/data/subjects/characteristics")]
        public async Task<Hashtable> getSubjectData(string studyId, [FromBody] List<int> SCids)
        {
            return await _dataService.getSubjectData(studyId, SCids);
        }

        [HttpPost]
        [Route("api/studies/{studyId}/data/clinical/observations")]
        public async Task<List<Hashtable>> getObservations(string studyId, [FromBody] List<int> observations)
        {
            return await _dataService.getObservationsData(studyId, observations);
        }

        
    }
}
