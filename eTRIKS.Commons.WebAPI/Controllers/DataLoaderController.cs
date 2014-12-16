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

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DataLoaderController : ApiController
    {
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
    }
}
