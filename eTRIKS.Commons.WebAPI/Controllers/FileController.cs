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
        [Route("study/{studyId}/upload")]
        public async Task<List<string>> UploadFile(string studyId)
        {
            try
            {
                //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
                string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
                string path = rawFilesDirectory + studyId;
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                if (Request.Content.IsMimeMultipartContent())
                {
                    var streamProvider = new StreamProvider(path);
                    await Request.Content.ReadAsMultipartAsync(streamProvider);
                    List<string> messages = new List<string>();
                    foreach (var file in streamProvider.FileData)
                    {
                        FileInfo fi = new FileInfo(file.LocalFileName);
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
        [Route("study/{studyId}/uploadedFiles")]
        public async Task<List<FileDTO>> GetUploadedFiles(string studyId)
        {
            string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            string path = rawFilesDirectory + studyId;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            return _fileService.getUploadedFiles(path);
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
