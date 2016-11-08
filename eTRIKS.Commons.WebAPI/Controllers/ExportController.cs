using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;

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
        [Route("api/projects/{projectAcc}/export/datafields")]
        public List<TreeNodeDTO> GetFields(string projectAcc)
        {
            var name = User.Identity.Name;
            return _exportService.GetAvailableFields(projectAcc);
        }
        [HttpPost]
        [Route("api/projects/{projectAcc}/export/datafields/valueset")]
        public async Task<DataFilterDTO> GetValueSet(string projectAcc, [FromBody] DataFieldDTO fieldDto)
        {
            return await _exportService.GetFieldValueSet(projectAcc, fieldDto);
        }

        [HttpPost]
        [Route("api/projects/{projectAcc}/export/table")]
        public async Task<Hashtable> GetDataPreview(string projectAcc, [FromBody] UserDatasetDTO userDatasetDto)
        {
            return await _exportService.ExportDataTable(projectAcc, userDatasetDto);
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
