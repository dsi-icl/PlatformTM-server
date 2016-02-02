using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using eTRIKS.Commons.DataParser.IOFileManagement;
using eTRIKS.Commons.Service.Services;


namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DataParserController : ApiController
    {

        private FileHandler _fileHandler;
        private TemplateService templateService;
        private FileService _fileService;

        public DataParserController(FileHandler fileHandler, TemplateService tmplService, FileService fileService)
        {
            _fileHandler = fileHandler;
            templateService = tmplService;
            _fileService = fileService;
        }


        [HttpGet]
        [Route("api/temp/loadtemp")]
        public void LoadTemplateFromFile()
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            string tempFilepath = "temp/BStemplate.csv";
            string tempVarFilepath = "temp/BStemplateVars.csv";
            templateService.loadDatasetTemplate(tempFilepath, tempVarFilepath);
        }

        [HttpGet]
        [Route("api/temp/matchids")]
        public void matchIds()
        {
            string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            //string tempFilepath = "temp/BStemplate.csv";
            //string tempVarFilepath = "temp/BStemplateVars.csv";
            _fileService.tempmethod();
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


        [HttpGet]
        public List<string> GetSourceFields(string dataSource)
        {
            IOUtility iOUtility = new IOUtility();
            return iOUtility.getDataSourceColumns(dataSource);
        }

        [HttpGet]
        public List<string> getWorkbooks(string fileName)
        {
            string fileLocation = @"C:\temp\" + fileName;
            IOUtility iOUtility = new IOUtility();
            return iOUtility.getExcelWorkbookNames(fileLocation);
        }

        [HttpGet]
        public List<string> getColumnHeadersInWorkbooks(string fileName, string workbook)
        {
            string fileLocation = @"C:\temp\" + fileName;
            IOUtility iOUtility = new IOUtility();
            return iOUtility.getExcelFiledsInWorkbook(fileLocation, workbook);
        }

        [HttpGet]
        public  string loadData(string dataSource, string fileName, string page, string mapping)
        {
            IOUtility iOUtility = new IOUtility();
            string ext = Path.GetExtension(fileName);
            if (ext == ".csv")
            {
                return  _fileHandler.loadDataFromFile(dataSource, iOUtility.readCSVFileContents(fileName, mapping));
            }
            else if (ext == ".xlsx")
            {
                return  _fileHandler.loadDataFromFile(dataSource, iOUtility.readExcelFileContents(fileName, page, mapping));
            }
            else if (ext == ".txt")
            {
                return  _fileHandler.loadDataFromFile(dataSource, iOUtility.readTabDelimitedFileContents(fileName, mapping));
            }

            return "ERROR: Cannot Parse File";
        }
    }
}
