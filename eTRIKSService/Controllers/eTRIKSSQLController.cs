using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKSService.Models;

namespace eTRIKSService.Controllers
{
    [RoutePrefix("api/etriksSQL")]
    public class eTRIKSSQLController : ApiController
    {
        [Route("")]
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("{Activity}")]
        // GET api/values
        public Activity GetActivity(string activity)
        {
            Activity act = new Activity();
            act.name = "Test Activity";
            act.OID = "A100002";
            return act;
        }
    }
}
