using System;
using System.Collections.Generic;
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

        public DataParserController(FileHandler fileHandler)
        {
            _fileHandler = fileHandler;
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

                File.WriteAllBytes(@"C:\temp\" + fileName, File.ReadAllBytes(fileLocalName));

                // Clean up App__Data folder 
                File.Delete(fileLocalName);

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
        public string loadData(string datSource, string fileName, string page, string mapping)
        {
            IOUtility iOUtility = new IOUtility();
            string ext = Path.GetExtension(fileName);
            if (ext == ".csv")
            {
                return _fileHandler.loadDataFromFile(datSource, iOUtility.readCSVFileContents(fileName, mapping));
            }
            else if (ext == ".xlsx")
            {
                return _fileHandler.loadDataFromFile(datSource, iOUtility.readExcelFileContents(fileName, page, mapping));
            }
            else if (ext == ".txt")
            {
                return _fileHandler.loadDataFromFile(datSource, iOUtility.readTabDelimitedFileContents(fileName, mapping));
            }

            return "ERROR: Cannot Parse File";
        }
    }
}
