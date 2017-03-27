using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.CodeGenerators;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Net.Http.Headers;
using MySql.Data.MySqlClient;

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

        ////////////[HttpGet("datasets/{datasetId}/preview")]
        ////////////public async Task<DataTable> GetDataPreview(string datasetId)
        ////////////{
        ////////////    return await _exportService.ExportDataset(datasetId); //.(projectId, userDatasetDto);
        ////////////}


        [Route("datasets/{datasetId}/IsFileReady")]
        [HttpGet]
        public bool IsFileReady(string datasetId)  
        {
            var status = _exportService.IsFileReady(datasetId);
            return status;
        }

        [Route("datasets/{datasetId}/download")]
        [HttpGet]
        public async void /*Task*/ PrepareFileForDownload(string datasetId)   // prepare the file in server
        {

            var filePrepared = await _exportService.ExportDataset(datasetId);   

            // TEMP as this should be called by front end seperately to download the file
            if (filePrepared)
                DownloadDatasets(datasetId);
        }
        
        [Route("datasets/{datasetId}/download1")]
        [HttpGet]
        public async void DownloadDatasets(string datasetId)  // file is ready download it 
        {
            string filename;
            var fileStream = _exportService.DownloadDataset( /*"55e87400-8968-4986-83e1-9527803250ce"*/ datasetId, out filename);
            
            HttpContext.Response.Clear();
            HttpContext.Response.ContentType = "text/csv";
            HttpContext.Response.Headers.Add("content-disposition", "attachment");
            HttpContext.Response.Headers.Add("x-filename", filename + ".csv");
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "x-filename");
            HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await fileStream.CopyToAsync(HttpContext.Response.Body);
            
            fileStream.Close();
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