using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class ExportController : Controller
    {
         private ExportService _exportService;

         public ExportController(ExportService exportService)
        {
            _exportService = exportService;
        }


        [HttpGet]
        [Route("api/projects/{projectId}/export/datafields")]
        public List<TreeNodeDTO> GetFields(int projectId)
        {
            var name = User.Identity.Name;
            return _exportService.GetAvailableFields(projectId);
        }
        [HttpPost]
        [Route("api/projects/{projectId}/export/datafields/valueset")]
        public  DataFilterDTO GetValueSet(int projectId, [FromBody] DataFieldDTO fieldDto)
        {
            return _exportService.GetFieldValueSet(projectId, fieldDto);
        }

        [HttpPost]
        [Route("api/projects/{projectId}/export/table")]
        public  Hashtable GetDataPreview(int projectId, [FromBody] UserDatasetDTO userDatasetDto)
        {
            return _exportService.ExportDataTable(projectId, userDatasetDto);
        }

        //[HttpPost]
        //[Route("api/projects/{projectAcc}/export/tree/")]
        //public async Task<List<TreeNodeDTO>> GetDataTree(string projectAcc, [FromBody] UserDatasetDTO userDatasetDto)
        //{
        //    return await _exportService.ExportDataTree(projectAcc, userDatasetDto);
        //}


        //[HttpGet]
        //[Route("api/export/test")]
        //public Task getSample()
        //{
        //    return _exportService.getFieldValueSet("P-BVS","VS[ORRES]");
        //}
    }
}
