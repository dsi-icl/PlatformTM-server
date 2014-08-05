using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace eTRIKSService.Controllers
{
    [RoutePrefix("api/etriks")]
    public class ValuesController : ApiController
    {
        //[Route("")]
        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //[Route("{id}")]
        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //[Route("{id}/{dd}")]
        //public string Get2(int id, string dd)
        //{
        //    string queryString = System.Web.HttpContext.Current.Request.Url.Query.ToString();
        //    return "value";
        //}

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
