using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    //[RoutePrefix("api/studies")]
    public class DataController : ApiController
    {
        //private ActivityService _activityService;

        //public DataController(ActivityService activityService)
        //{
        //    _activityService = activityService;
        //}

        [HttpPost]
        [Route("api/studies/{studyId}/data/clinical/observations")]
        public string getObservations(string studyId, [FromBody] List<string> observations)
        {
            Console.Out.WriteLine(studyId);

            Console.Out.WriteLine(observations);
            return "OK";
        }

    }
}
