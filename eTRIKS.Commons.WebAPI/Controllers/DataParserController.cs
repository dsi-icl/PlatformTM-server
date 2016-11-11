﻿using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DataParserController : Controller
    {

        //private FileHandler _fileHandler;
        private TemplateService templateService;
        private FileService _fileService;

        public DataParserController(TemplateService tmplService, FileService fileService)
        {
            //_fileHandler = fileHandler;
            templateService = tmplService;
            _fileService = fileService;
        }

        [HttpGet]
        [Route("api/temp/loadDatamatrixtemplate")]
        public void loadmatrixTemp()
        {
            templateService.loadDataMatrixTemplate();
        }

        [HttpGet]
        [Route("api/temp/loadtemp")]
        public void LoadTemplateFromFile()
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            string tempFilepath = "temp/dataTemplates/REtemplate.csv";
            string tempVarFilepath = "temp/dataTemplates/REtemplateVars.csv";
            templateService.loadDatasetTemplate(tempFilepath, tempVarFilepath);
        }

        [HttpGet]
        [Route("api/temp/matchids")]
        public void matchIds()
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/BStemplate.csv";
            //string tempVarFilepath = "temp/BStemplateVars.csv";
           // _fileService.tempmethod();
        }

        [HttpGet]
        [Route("api/temp/loadTerms")]
        public void LoadTerms()
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            string tempFilepath = "temp/assaytermsDict.csv";
            string tempVarFilepath = "temp/assaytermsCVs.csv";
            templateService.loadCVterms(tempFilepath, tempVarFilepath);
        }

        [HttpGet]
        [Route("api/temp/widetolong")]
        public void widetolong()
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/BStemplate.csv";
            //string tempVarFilepath = "temp/BStemplateVars.csv";
           // _fileService.getLongFormat2();
        }


        [HttpPost]
        public async Task<HttpResponseMessage> LoadFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                string fileName = "";
                string fileLocalName = "";
                var result = await Request.Content.ReadAsMultipartAsync(provider);
                foreach (MultipartFileData file in provider.FileData)
                {
                    fileName = file.Headers.ContentDisposition.FileName.Trim('"');
                    fileLocalName = file.LocalFileName;
                }
                string ext = Path.GetExtension(fileName);

                //File.WriteAllBytes(@"C:\temp\" + fileName, File.ReadAllBytes(fileLocalName));

                // Clean up App__Data folder 
                //File.Delete(fileLocalName);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }


        //[HttpGet]
        //public List<string> GetSourceFields(string dataSource)
        //{
        //    IOUtility iOUtility = new IOUtility();
        //    return iOUtility.getDataSourceColumns(dataSource);
        //}

        //[HttpGet]
        //public List<string> getWorkbooks(string fileName)
        //{
        //    string fileLocation = @"C:\temp\" + fileName;
        //    IOUtility iOUtility = new IOUtility();
        //    return iOUtility.getExcelWorkbookNames(fileLocation);
        //}

        //[HttpGet]
        //public List<string> getColumnHeadersInWorkbooks(string fileName, string workbook)
        //{
        //    string fileLocation = @"C:\temp\" + fileName;
        //    IOUtility iOUtility = new IOUtility();
        //    return iOUtility.getExcelFiledsInWorkbook(fileLocation, workbook);
        //}

        //[HttpGet]
        //public  string loadData(string dataSource, string fileName, string page, string mapping)
        //{
        //    IOUtility iOUtility = new IOUtility();
        //    string ext = Path.GetExtension(fileName);
        //    if (ext == ".csv")
        //    {
        //        return  _fileHandler.loadDataFromFile(dataSource, iOUtility.readCSVFileContents(fileName, mapping));
        //    }
        //    else if (ext == ".xlsx")
        //    {
        //        return  _fileHandler.loadDataFromFile(dataSource, iOUtility.readExcelFileContents(fileName, page, mapping));
        //    }
        //    else if (ext == ".txt")
        //    {
        //        return  _fileHandler.loadDataFromFile(dataSource, iOUtility.readTabDelimitedFileContents(fileName, mapping));
        //    }

        //    return "ERROR: Cannot Parse File";
        //}
    }
}
