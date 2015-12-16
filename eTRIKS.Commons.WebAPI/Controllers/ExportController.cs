using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class ExportController : ApiController
    {
         private ExportService _exportService;

         public ExportController(ExportService exportService)
        {
            _exportService = exportService;
        }

         [HttpPost]
         [Route("api/export/{projectId}")]
         public async Task<List<Subject>>  getSubjectData(string projectId, [FromBody] ExportRequestDTO dto)
         {
              return await _exportService.ExportDatasets(projectId,dto);
         }

        //[HttpGet]
        //[Route("api/export/sample")]
        //public ExportRequestDTO getSample()
        //{
        //    return _exportService.getSampleRequest();
        //}
    }
}
