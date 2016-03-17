using System.Collections;
using System.Configuration;
using System.Data;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/files")]
    public class FileController : ApiController
    {
         private FileService _fileService;

        public FileController(FileService fileService)
        {
            _fileService = fileService;
        }


        [HttpPost]
        [Route("project/{projectId}/createdir")]
        public List<string> CreateDirectory(string projectId, [FromBody] DirectoryDTO dir)
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            string projDir = fileDir + projectId;
            string newDir = projDir  +"/"+dir.name;
            
             var diInfo =    _fileService.addDirectory(projectId, newDir);

            return diInfo == null ? null : diInfo.GetDirectories().Select(d => d.Name).ToList();
        }

        [HttpGet]
        [Route("project/{projectId}/directories")]
        public List<string> getDirectories(string projectId)
        {
            return _fileService.getDirectories(projectId);
            //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string projDir = fileDir + projectId;
            ////string newDir = projDir + "/" + dirName;
            //if (!Directory.Exists(projDir)) Directory.CreateDirectory(projDir);
            //return new DirectoryInfo(projDir).GetDirectories().Select(d => d.Name).ToList();
        }

        [HttpPost]
        [Route("project/{projectId}/upload/{dir}")]
        public async Task<List<string>> UploadFile(string projectId, string dir)
        {
            try
            {
                //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
                string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
                string path = rawFilesDirectory + projectId + "\\" + dir  ;
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
        [Route("project/{projectId}/uploadedFiles/{subdir}")]
        public async Task<List<FileDTO>> GetUploadedFiles(string projectId,string subdir)
        {
            string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            //string path = rawFilesDirectory + projectId;
            string relativePath = projectId; 
            //if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            if(!subdir.Equals("top"))
                relativePath = relativePath + "\\" + subdir.Replace('_','\\');

            return _fileService.getUploadedFiles(projectId, relativePath);
        }

        [HttpGet]
        [Route("project/{projectId}/preview/{fileId}")]
        public async Task<Hashtable> getDatasetPreview(int fileId)
        {
            return _fileService.getFilePreview(fileId);
        }

        //[HttpGet]
        //[Route("{fileName}/fileHeader")]
        //public async Task<List<Dictionary<string, string>>> LinkFile(string fileName)
        //{

        //    //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
        //    var rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
        //    string path = rawFilesDirectory + studyId;
        //    string filePath = rawFilesDirectory + "\\" + fileName;
        //    return _fileService.getFileColHeaders(filePath);
        //}

        
    }
}
