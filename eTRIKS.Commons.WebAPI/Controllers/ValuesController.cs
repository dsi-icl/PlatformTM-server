using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        [Authorize("Bearer")]
        public string Get()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var id = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "ID").Value;

            return $"Hello! {HttpContext.User.Identity.Name}, your ID is:{id}";
        }
    }
}


//previous version

//////using System;
//////using System.Collections.Generic;
//////using System.Linq;
//////using System.Threading.Tasks;
//////using Microsoft.AspNetCore.Mvc;
//////using Microsoft.AspNetCore.Authorization;

//////namespace eTRIKS.Commons.WebAPI.Controllers
//////{
//////    [Route("api/[controller]")]
//////    public class ValuesController : Controller
//////    {
//////        // GET: api/values
//////        [HttpGet]
//////        public IEnumerable<string> Get()
//////        {
//////            //throw new Exception("Horsed");
//////            return new string[] { "value1", "value2" };
//////        }

//////        // GET api/values/5
//////        [HttpGet("{id}")]
//////        [Authorize("Bearer")]
//////        public string Get(int id)
//////        {
//////            return "value";
//////        }
//////    }
//////}
