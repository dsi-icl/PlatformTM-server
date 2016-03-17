using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.WebAPI.Controllers
{
  //  [Authorize]
    public class AssayController : ApiController
    {
         private readonly AssayService _assayService;

         public AssayController(AssayService assayService)
        {
            _assayService = assayService;
        }

         [HttpGet]
         [Route("api/projects/{projectAcc}/assays")]
         public List<AssayDTO> GetAssays(string projectAcc)
         {
             return _assayService.GetProjectAssays(projectAcc);
         }

        [HttpGet]
        [Route("api/projects/{projectId}/assays/{assayId}/samples")]
        public Task<Hashtable> GetSamplesData(string projectId, int assayId)
        {
            return _assayService.GetSamplesDataPerAssay(projectId, assayId);
        }
    }
}
