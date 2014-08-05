using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eTRIKSService.Controllers
{
    [RoutePrefix("api/etriksNOSQL")]
    public class eTRIKSNOSQLController : ApiController
    {
        [Route("")]
        // GET api/values
        public IEnumerable<string> getRecordsOfStudy()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("{id}")]
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }
    }
}
