using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Authorize]
    public class AssayController : Controller
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

        [HttpGet]
        [Route("api/assays/temp/{assayId}/loadPA/{fileId}")]
        public HttpResponseMessage AddPA(int assayId, int fileId)
        {
            _assayService.addPA(assayId, fileId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
