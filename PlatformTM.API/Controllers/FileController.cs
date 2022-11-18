using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;
using PlatformTM.Models.Services.HelperService;
using PlatformTM.Models.ViewModels;

namespace PlatformTM.API.Controllers
{
    [Route("file-management")]
    public class FileController : Controller
    {
        private readonly FileService _fileService;
       

        public FileController(FileService fileService)
        {
            _fileService = fileService;
         
          
        }

        [HttpGet("files/{fileId}", Name = "GetFileById")]
        public FileDTO GetFile(int fileId)
        {
            return _fileService.GetFile(fileId);
        }

        [HttpGet]
        [Route("projects/{projectId}/drive/folders")]
        public List<string> GetDirectories(int projectId)
        {
            return _fileService.GetDirectories(projectId);
        }


        [HttpGet("projects/{projectId}/drive/{dirId?}")]
        public DriveVM GetUploadedFiles(int projectId, int dirId)
        {
            //Check here for permissions
            return _fileService.GetFiles(projectId, dirId);
        }

        [HttpGet("files/{fileId}/preview")]
        public IActionResult GetFilePreview(int fileId)
        {
            try
            {
                var dt = _fileService.GetFilePreview(fileId);
                return new OkObjectResult(dt);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Route("files/{fileId}/download")]
        [HttpGet]
        public async Task<ActionResult> DownloadFile(int fileId)
        {
            string filename;
            var fileStream = _fileService.GetFile(fileId, out filename);
            if (fileStream == null) return NotFound("cannot file ddlkjaskjh ");

            HttpContext.Response.Clear();
            Response.ContentType = "application/octet-stream";
            Response.Headers.Add("content-disposition", "attachment");
            Response.Headers.Add("x-filename", filename + ".csv");
            Response.Headers.Add("content-length", fileStream.Length.ToString());
            Response.Headers.Add("Access-Control-Expose-Headers", "x-filename , content-length, content-disposition");
            Response.Headers.Add("Access-Control-Allow-Origin", "*");
            await fileStream.CopyToAsync(HttpContext.Response.Body);
            fileStream.Dispose();
            //fileStream.Close();
            return new FileStreamResult(fileStream, "text/csv") { FileDownloadName = filename + ".csv" };
        }


        //[HttpGet("{datasetId}/download")]
        //[AllowAnonymous]
        //public async Task<IActionResult> DownloadDataset(int datasetId)
        //{
        //    //var datatable = await _datasetService.ConsolidateDataset(datasetId);

        //    //byte[] outputBuffer = null;

        //    //using (MemoryStream tempStream = new MemoryStream())
        //    //{
        //    //    using (StreamWriter writer = new StreamWriter(tempStream))
        //    //    {
        //    //        IOhelper.WriteDataTable(datatable, writer, true);
        //    //    }

        //    //    outputBuffer = tempStream.ToArray();
        //    //}
        //    ////Response.Headers.Add("Content-Disposition", "inline; filename="+datatable.TableName + ".csv");
        //    //return File(outputBuffer, "text/csv", datatable.TableName + ".csv");

        //}

        [HttpPost("projects/{projectId}/drive/folders")]
        public IActionResult CreateDirectory(int projectId, [FromBody] DirectoryDTO dir)
        {
            var folderInfo = _fileService.CreateFolder(projectId, dir);
            return new CreatedAtRouteResult("GetFileById", new { fileId = folderInfo.Id }, folderInfo);
        }

        [HttpPost("projects/{projectId}/upload/{dirId?}")]
        public async Task<IActionResult> UploadFile(int projectId, int dirId)
        {
            try
            {
                var path = _fileService.GetFullPath(projectId);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                if (Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length <= 0) return BadRequest("File size is zero");
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);

                    }
                    var fi = new FileInfo(Path.Combine(path, file.FileName));
                    _fileService.AddOrUpdateFile(projectId, fi, dirId);
                    return Ok();
                }
                return BadRequest($"Expected a multipart request, but got '{Request.ContentType}'");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpGet]
        [Route("{fileId}/remove")]
        public async void DeleteFile(int fileId)
        {
            //if(await _datasetService.UnloadFileDatasets(fileId))
            //_fileService.DeleteFile(fileId);
        }

        
    }
}
