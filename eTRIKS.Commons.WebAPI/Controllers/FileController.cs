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
    //[RoutePrefix("api/files")]
    public class FileController : Controller
    {
         private FileService _fileService;
        private IHostingEnvironment _environment;

        public FileController(FileService fileService, IHostingEnvironment env)
        {
            _fileService = fileService;
            _environment = env;
        }

        [HttpGet]
        [Route("api/files/{fileId}")]
        public FileDTO GetFile(int fileId)
        {
            return _fileService.GetFileDTO(fileId);
        }

        [HttpPost]
        [Route("api/files/projects/{projectId}/createdir")]
        public List<string> CreateDirectory(int projectId, [FromBody] DirectoryDTO dir)
        {

            var fullpath = _fileService.GetFullPath(projectId.ToString(), dir.name);
            
            var diInfo =    _fileService.addDirectory(projectId, fullpath);
            
            return diInfo == null ? null : diInfo.GetDirectories().Select(d => d.Name).ToList();
        }

        [HttpGet]
        [Route("api/files/projects/{projectId}/directories")]
        public List<string> getDirectories(int projectId)
        {
            return _fileService.getDirectories(projectId);
        }

        //[HttpPost]
        //[Route("api/files/projects/{projectId}/upload/{dir?}")]
        //public async IActionResult UploadFile(int projectId, string dir="")
        //{
        //    try
        //    {
        //        //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
        //        string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
        //        string path = rawFilesDirectory + "P-"+ projectId + "\\" + dir  ;
        //        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        //        if (Request.ContentType.IndexOf("multipart/",StringComparison.OrdinalIgnoreCase) >=0)
        //        {
        //            var streamProvider = new StreamProvider(path);

        //            await Request.Form.Files[0].ReadAsMultipartAsync(streamProvider);
        //            List<string> messages = new List<string>();
        //            foreach (var file in streamProvider.FileData)
        //            {
        //                FileInfo fi = new FileInfo(file.LocalFileName);

        //                if (_fileService.addOrUpdateFile(projectId, fi)==null)
        //                    throw new Exception("Failed to updated database");
        //                messages.Add("File uploaded as " + fi.FullName + " (" + fi.Length + " bytes)");
        //            }

        //            return messages;
        //        }
        //        else
        //        {
        //            return BadRequest($"Expected a multipart request, but got '{Request.ContentType}'");
        //            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
        //            //throw new HttpResponseException(response);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        [HttpPost]
        [Route("api/files/projects/{projectId}/upload/{dir?}")]
        public async Task<IActionResult> UploadFile(int projectId, ICollection<IFormFile> files, string dir = "")
        {
            try
            {
                //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
                //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
                //string path = rawFilesDirectory + "P-" + projectId + "\\" + dir;

                string path = _fileService.GetFullPath(projectId.ToString(), dir);
                //var path2 = Path.Combine(_environment.WebRootPath)


                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    //var streamProvider = new StreamProvider(path);

                    //await Request.Form.Files[0].ReadAsMultipartAsync(streamProvider);
                    //List<string> messages = new List<string>();
                    //foreach (var file in streamProvider.FileData)
                    //{
                    //    FileInfo fi = new FileInfo(file.LocalFileName);

                    //    if (_fileService.addOrUpdateFile(projectId, fi) == null)
                    //        throw new Exception("Failed to updated database");
                    //    messages.Add("File uploaded as " + fi.FullName + " (" + fi.Length + " bytes)");
                    //}

                    //return messages;

                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                            FileInfo fi = new FileInfo(file.FileName);
                            _fileService.addOrUpdateFile(projectId, fi);
                        }
                    }
                    return Ok();
                }
                else
                {
                    return BadRequest($"Expected a multipart request, but got '{Request.ContentType}'");
                    //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    //throw new HttpResponseException(response);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet]
        [Route("api/files/projects/{projectId}/uploadedFiles/{subdir?}")]
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

            return _fileService.getUploadedFiles(projectId, relativePath);
        }

        [HttpGet]
        [Route("api/files/{fileId}/preview")]
        public Hashtable getDatasetPreview(int fileId)
        {
            return _fileService.getFilePreview(fileId);
        }
        
    }
}
