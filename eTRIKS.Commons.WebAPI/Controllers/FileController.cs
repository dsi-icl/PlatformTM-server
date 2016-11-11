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

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/files")]
    public class FileController : Controller
    {
         private FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        [Route("{fileId}")]
        public FileDTO GetFile(int fileId)
        {
            return _fileService.GetFileDTO(fileId);
        }

        [HttpPost]
        [Route("projects/{projectId}/createdir")]
        public List<string> CreateDirectory(int projectId, [FromBody] DirectoryDTO dir)
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            string projDir = fileDir + "P-" + projectId;
            string newDir = projDir  +"/"+dir.name;
            
             var diInfo =    _fileService.addDirectory(projectId, newDir);
            
            return diInfo == null ? null : diInfo.GetDirectories().Select(d => d.Name).ToList();
        }

        [HttpGet]
        [Route("projects/{projectId}/directories")]
        public List<string> getDirectories(int projectId)
        {
            return _fileService.getDirectories(projectId);
        }

        [HttpPost]
        [Route("projects/{projectId}/upload/{dir?}")]
        public async Task<List<string>> UploadFile(int projectId, string dir="")
        {
            try
            {
                //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
                string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
                string path = rawFilesDirectory + "P-"+ projectId + "\\" + dir  ;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (Request.Content.IsMimeMultipartContent())
                {
                    var streamProvider = new StreamProvider(path);
                   
                    await Request.Content.ReadAsMultipartAsync(streamProvider);
                    List<string> messages = new List<string>();
                    foreach (var file in streamProvider.FileData)
                    {
                        FileInfo fi = new FileInfo(file.LocalFileName);

                        if (_fileService.addOrUpdateFile(projectId, fi)==null)
                            throw new Exception("Failed to updated database");
                        messages.Add("File uploaded as " + fi.FullName + " (" + fi.Length + " bytes)");
                    }
                    
                    return messages;
                }
                else
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Request!");
                    throw new HttpResponseException(response);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpGet]
        [Route("projects/{projectId}/uploadedFiles/{subdir?}")]
        public async Task<List<FileDTO>> GetUploadedFiles(int projectId,string subdir="")
        {
            string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            //string path = rawFilesDirectory + projectId;
            string relativePath = "P-"+projectId; 
            //if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if(subdir != "")
                relativePath = relativePath + "\\" + subdir.Replace('_','\\');

            return _fileService.getUploadedFiles(projectId, relativePath);
        }

        [HttpGet]
        [Route("{fileId}/preview")]
        public async Task<Hashtable> getDatasetPreview(int fileId)
        {
            return _fileService.getFilePreview(fileId);
        }
        
    }
}
