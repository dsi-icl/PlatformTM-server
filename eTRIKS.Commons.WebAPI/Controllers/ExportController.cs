using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
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
         public async Task<List<HumanSubject>>  getSubjectData(string projectId, [FromBody] ExportRequestDTO dto)
         {
              return await _exportService.ExportDatasets(projectId,dto);
         }

        [HttpGet]
        [Route("api/projects/{projectAcc}/export/datafields")]
        public List<TreeNodeDTO> GetFields(string projectAcc)
        {
            return _exportService.getAvailableFields(projectAcc);
        }
        [HttpPost]
        [Route("api/projects/{projectAcc}/export/datafields/valueset")]
        public async Task<Field> GetValueSet(string projectAcc, [FromBody] Field field)
        {
            return await _exportService.getFieldValueSet(projectAcc,field);
        }

        [HttpPost]
        [Route("api/projects/{projectAcc}/export/table")]
        public async Task<Hashtable> GetDataPreview(string projectAcc, [FromBody] List<Criterion> criteria)
        {
            return await _exportService.ExportDataTable(projectAcc, criteria);
        }

        [HttpPost]
        [Route("api/projects/{projectAcc}/export/tree/")]
        public async Task<List<TreeNodeDTO>> GetDataTree(string projectAcc, [FromBody] List<Criterion> criteria)
        {
            return await _exportService.ExportDataTree(projectAcc, criteria);
        }


        //[HttpGet]
        //[Route("api/export/test")]
        //public Task getSample()
        //{
        //    return _exportService.getFieldValueSet("P-BVS","VS[ORRES]");
        //}
    }
}
