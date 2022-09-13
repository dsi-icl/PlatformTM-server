using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;
using PlatformTM.Models.ViewModels;

namespace PlatformTM.API.Controllers
{
    [Route("files")]
    public class FileController : Controller
    {
        private readonly FileService _fileService;
        private readonly DatasetService _datasetService;

        public FileController(FileService fileService, DatasetService datasetService)
        {
            _fileService = fileService;
            _datasetService = datasetService;
          
        }

        [HttpGet("{fileId}", Name = "GetFileById")]
        public FileDTO GetFile(int fileId)
        {
            return _fileService.GetFile(fileId);
        }

        [HttpGet]
        [Route("{fileId}/initload/datasets/{datasetId}")]
        public bool InitLoadFile(int fileId, int datasetId)
        {
            return _fileService.InitLoading(fileId, datasetId);
            //if (success)
            //    return Ok(true);
            //return new BadRequestResult();
        }

        [HttpGet]
        [Route("{fileId}/load/datasets/{datasetId}")]
        public void LoadFile(int fileId, int datasetId)
        {
            var success = _fileService.LoadFile(fileId, datasetId);
            //if (success)
            //    return Ok(true);
            //return new BadRequestResult();
        }

        [HttpGet]
        [Route("{fileId}/progress")]
        public IActionResult GetLoadingProgress(int fileId)
        {
            var progressDTO = _fileService.GetFile(fileId);
            return new OkObjectResult(progressDTO);
        }

        [HttpGet]
        [Route("{fileId}/unload")]
        public async void UnloadFile(int fileId)
        {
            await _datasetService.UnloadFileDatasets(fileId);
        }

        [HttpGet]
        [Route("{fileId}/remove")]
        public async void DeleteFile(int fileId)
        {
            if(await _datasetService.UnloadFileDatasets(fileId))
            _fileService.DeleteFile(fileId);
        }

        [HttpGet("{fileId}/preview")]
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

        [HttpPost]
        [Route("{fileId}/mapToTemplate/datasets/{datasetId}")]
        public int? MapToTemplate(int datasetId, int fileId, [FromBody] DataTemplateMap dataTemplateMap)
        {
            return _fileService.mapToTemplate(datasetId, fileId, dataTemplateMap);
        }

        [HttpGet("{fileId}/match/datasets/{datasetId}")]
        public FileDTO CheckValidTemplate(int datasetId, int fileId)
        {
            return _fileService.MatchFileToTemplate(datasetId, fileId);
        }

        [HttpPost("projects/{projectId}/folder/create")]
        public IActionResult CreateDirectory(int projectId, [FromBody] DirectoryDTO dir)
        {
            var folderInfo =    _fileService.CreateFolder(projectId, dir);
            return new CreatedAtRouteResult("GetFileById", new { fileId = folderInfo.Id }, folderInfo);
        }

        [HttpGet]
        [Route("projects/{projectId}/drive/directories")]
        public List<string> GetDirectories(int projectId)
        {
            return _fileService.GetDirectories(projectId);
        }

        [HttpPost("projects/{projectId}/upload/{dirId?}")]
        public async Task<IActionResult> UploadFile(int projectId,  int dirId)
        {
            try
            {
                var path = _fileService.GetFullPath(projectId);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                if (Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var file= Request.Form.Files[0];
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

        [HttpGet("projects/{projectId}/drive/{dirId?}")]
        public  DriveVM GetUploadedFiles(int projectId,int dirId)
        {
            //Check here for permissions
            return _fileService.GetFiles(projectId, dirId);
        }

        [Route("{fileId}/download")]
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
    }
}
