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
        public bool IsFileReady(string datasetId)  // continous check to see if file is ready 
        {
            var status = _exportService.IsFileReady(datasetId);
            return status;
        }

        [Route("datasets/{datasetId}/download")]
        [HttpGet]
        public async void /*Task*/ PrepareFileForDownload(string datasetId)   // prepare the file in server
        {

            var filePrepared = await _exportService.ExportDataset(datasetId);   // saves the file to the server

            // TEMP as this should be called by front end seperately to download the file
            if (filePrepared)
                DownloadDatasets(datasetId);
        }
        
        [Route("datasets/{datasetId}/download1")]
        [HttpGet]
        public async void /*Task*/  DownloadDatasets(string datasetId)  // file is ready download it 
        {
            string filename;
            var fileStream = _exportService.DownloadDataset(datasetId/*"55e87400-8968-4986-83e1-9527803250ce"*/, out filename);

            // FOR 2 SAMPLES
            // sUBJECT DATA  "2a72d213-dadb-4c1d-8ae5-46bf8cea762a"
            // Gene expression samples "b2b1ccc3-dee4-41f0-856e-9af6ff66f573"
            // assay data "55e87400-8968-4986-83e1-9527803250ce"

            // datasetid for subject metadata "147fcd65-89bf-4904-9939-fe9c18034137"
            // datasetid for Gene Expression Assay "5cca7964-6756-4e29-acab-1806ed94fefb"
            // "f42a681a-bdaf-4bc5-99d2-0ead863fe93b"

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






//////////////[HttpGet("datasets/{datasetId}/preview")]
//////////////public async Task<DataTable> GetDataPreview(string datasetId)
//////////////{
//////////////    return await _exportService.ExportDataset(datasetId); //.(projectId, userDatasetDto);
//////////////}


//////////////[Route("datasets/{datasetId}/download")]
//////////////[HttpGet]
//////////////public async void /*Task*/ DownloadDatasets(string datasetId)
//////////////{

//////////////    // var dtTable =  await Task.Run(() => _exportService.ExportDataset(datasetId));


//////////////    var dtTable = await _exportService.ExportDataset(datasetId);

//////////////    await Task.Run(() =>
//////////////    {

//////////////        // var dtTable = await _exportService.ExportDataset(datasetId);
//////////////        // trick to get the file name
//////////////        string fileName = dtTable.TableName;
//////////////        var csvFile = _exportService.DownloadDataset(dtTable);


//////////////        HttpContext.Response.Clear();
//////////////        HttpContext.Response.ContentType = "text/csv";
//////////////        HttpContext.Response.Headers.Count();
//////////////        HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName + ".csv");
//////////////        HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
//////////////        HttpContext.Response.WriteAsync(csvFile);
//////////////    });
//////////////    // return true;
//////////////}