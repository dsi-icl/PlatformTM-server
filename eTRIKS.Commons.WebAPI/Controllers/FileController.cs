using System.Collections;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("files")]
    public class FileController : Controller
    {
        private readonly FileService _fileService;
        private IHostingEnvironment _environment;

        public FileController(FileService fileService, IHostingEnvironment env)
        {
            _fileService = fileService;
            _environment = env;
        }

        [HttpGet("{fileId}")]
        public FileDTO GetFile(int fileId)
        {
            return _fileService.GetFileDTO(fileId);
        }

        [HttpGet]
        [Route("{fileId}/remove")]
        public void DeleteFile(int fileId)
        {
            _fileService.DeleteFile(fileId);
        }

        [HttpGet("{fileId}/preview")]
        public Hashtable GetFilePreview(int fileId)
        {
            return _fileService.getFilePreview(fileId);
        }

        [HttpPost("projects/{projectId}/createdir")]
        public List<string> CreateDirectory(int projectId, [FromBody] DirectoryDTO dir)
        {
            var fullpath = _fileService.GetFullPath(projectId.ToString(), dir.name);
            var diInfo =    _fileService.addDirectory(projectId, fullpath);
            return diInfo?.GetDirectories().Select(d => d.Name).ToList();
        }

        [HttpGet]
        [Route("projects/{projectId}/directories")]
        public List<string> GetDirectories(int projectId)
        {
            return _fileService.getDirectories(projectId);
        }

        [HttpPost("projects/{projectId}/upload/{dir?}")]
        public async Task<IActionResult> UploadFile(int projectId,  string dir = "")
        {
            try
            {
                var path = _fileService.GetFullPath(projectId.ToString(), dir);
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
                    _fileService.addOrUpdateFile(projectId, fi);
                    return Ok();
                }
                return BadRequest($"Expected a multipart request, but got '{Request.ContentType}'");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("projects/{projectId}/uploadedFiles/{subdir?}")]
        public  List<FileDTO> GetUploadedFiles(int projectId,string subdir="")
        {
            //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            //string path = rawFilesDirectory + projectId;
            //string relativePath = "P-"+projectId; 
            //if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            //if(subdir != "")
            //    relativePath = relativePath + "\\" + subdir.Replace('_','\\');
            string relativePath = Path.Combine("P-" + projectId, subdir?.Replace('_', '\\'));
            //_fileService.GetFullPath(projectId.ToString(), subdir?.Replace('_', '\\'));
            //    _fileService.GetProjectPath()

            return _fileService.GetUploadedFiles(projectId, relativePath);
        }
  
    }
}
