using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Service.Services;
using System.Collections;
using System.Threading.Tasks;

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

        [HttpPost]
        [Route("api/studies/{studyId}/data/clinical/{domainCode}/observations")]
        public List<Hashtable> getObservations(string studyId, string domainCode, [FromBody] List<string> observations)
        {

            return _dataService.getObservationsData(studyId, domainCode, observations);
        }

        [HttpPost]
        [Route("api/studies/{studyId}/data/subjects/characteristics")]
        public async Task<List<Hashtable>> getSubjectData(string studyId, [FromBody] List<string> characs)
        {
            return await _dataService.getSubjectData(studyId, characs);
        }

        /*[HttpGet]
        [Route("api/studies/{studyId}/data/subjects/characteristics")]
        public List<Hashtable> getSubjectData(string studyId)
        {
            List<string> characs = new List<string>();
            characs.Add("AGE");
            characs.Add("SEX");
            characs.Add("RACE");
            characs.Add("ETHNIC");
            characs.Add("ARM");
            return _dataService.getSubjectData(studyId, characs);
        }*/

        [HttpGet]
        [Route("api/studies/{studyId}/data/clinical/observations")]
        public void getObservations(string studyId)
        {
             _dataService.getObservationsData(studyId,null);
        }

        
    }
}
