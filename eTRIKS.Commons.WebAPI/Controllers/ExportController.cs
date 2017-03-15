using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("apps/export")]
    public class ExportController : Controller
    {
         private readonly ExportService _exportService;

         public ExportController(ExportService exportService)
        {
            _exportService = exportService;
        }

        [HttpGet("datasets/{datasetId}/preview")]
        public DataTable GetDataPreview(string datasetId)
        {
            return _exportService.ExportDataset(datasetId);//.(projectId, userDatasetDto);
        }


        [Route("datasets/{datasetId}/download")]
        [HttpGet]
        public void DownloadDatasets(string datasetId)
        {

            var dtTable = _exportService.ExportDataset(datasetId);
            // trick to get the file name
            string fileName = dtTable.TableName;
            var csvFile = _exportService.DownloadDataset(dtTable);


            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.Headers.Count();
            HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName + ".csv");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            HttpContext.Response.WriteAsync(csvFile);

        }

        //[HttpGet]
        //[Route("projects/{projectId}/datafields")]
        //public List<TreeNodeDTO> GetFields(int projectId)
        //{
        //    var name = User.Identity.Name;
        //    return _exportService.GetAvailableFields(projectId);
        //}
        //[HttpPost]
        //[Route("projects/{projectId}/datafields/valueset")]
        //public  DataFilterDTO GetValueSet(int projectId, [FromBody] DataFieldDTO fieldDto)
        //{
        //    return _exportService.GetFieldValueSet(projectId, fieldDto);
        //}

        //[HttpPost]
        //[Route("projects/{projectId}/preview")]
        //public  Hashtable GetDataPreview(int projectId, [FromBody] UserDatasetDTO userDatasetDto)
        //{
        //    return _exportService.ExportDataTable(projectId, userDatasetDto);
        //}

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
