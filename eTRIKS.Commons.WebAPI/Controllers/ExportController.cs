using System;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("apps/export")]
    public class ExportController : Controller
    {
        private readonly ExportService _exportService;
        private readonly FileService _fileService;

        public ExportController(ExportService exportService, FileService fileService)
        {
            _exportService = exportService;
            _fileService = fileService;
        }

        [HttpGet("datasets/{datasetId}/preview")]
        public async Task<IActionResult> GetDataPreview(string datasetId)
        {
            try
            {
                var dt = await _exportService.ExportDataset(datasetId);
                return new OkObjectResult(dt);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [Route("datasets/{datasetId}/IsFileReady")]
        [HttpGet]
        public int IsFileReady(string datasetId)  
        {
            var status = _exportService.IsFileReady(datasetId);
            return status;
        }

        
        [Route("datasets/{datasetId}/export")]
        [HttpGet]
        public async void  PrepareFileForDownload2(string datasetId) // prepare the file in server
        {
           try
            {
                _exportService.SetDatasetStatus(datasetId, "", 1);
                var filePath = _exportService.GetDownloadPath(datasetId);
                var dataset = _exportService.GetDataset(datasetId);

                if (dataset.Type == "ASSAY")
                {
                    var fileInfo = await _exportService.ExportDatasetForAssay(dataset, filePath);
                    _exportService.SetDatasetStatus(datasetId, fileInfo.FullName, 2);
                }

                else
                {
                    var dt = await _exportService.ExportDataset(datasetId);
                    var fileInfo = _fileService.WriteDataFile(filePath, dt);  
                    _exportService.SetDatasetStatus(datasetId, fileInfo.FullName, 2);
                }
            }

            catch (Exception e)
            {
                throw;
            }
        }

        [Route("datasets/{datasetId}/download")]
        [HttpGet]
        public async Task<ActionResult> DownloadDatasets(string datasetId)
        {
           string filename;
            var fileStream = _exportService.DownloadDataset(datasetId, out filename);
            if (fileStream == null) return NotFound("cannot file ddlkjaskjh ");
            
            HttpContext.Response.Clear();
            Response.ContentType = "text/csv";
            Response.Headers.Add("content-disposition", "attachment");
            Response.Headers.Add("x-filename", filename + ".csv");
            Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await fileStream.CopyToAsync(HttpContext.Response.Body);
            
            fileStream.Close(); 
            return new FileStreamResult(fileStream,"text/csv") {FileDownloadName = filename+".csv"};
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