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
    public class FileUploadController : ApiController
    {
        [HttpPost]
        public async Task<List<string>> LoadFile()
        {
            try
            {
                string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
                if (Request.Content.IsMimeMultipartContent())
                {
                    var streamProvider = new StreamProvider(PATH);
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
    }
}
