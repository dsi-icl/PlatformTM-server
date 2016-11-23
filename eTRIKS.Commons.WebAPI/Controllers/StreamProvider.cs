using System.Net.Http.Headers;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    //public class StreamProvider : MultipartFormDataStreamProvider
    //{
    //    public StreamProvider(string path) : base(path)
    //    {}
    //    public override string GetLocalFileName(HttpContentHeaders headers)
    //    {
    //        // Check that we have a content type
    //        if (headers != null && headers.ContentType != null)
    //        {
    //            MediaTypeHeaderValue contentType = headers.ContentType;
                
    //            var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
    //            return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotatio

    //        }
    //        else
    //        {
    //            // Default to base behavior
    //            return base.GetLocalFileName(headers);
    //        }
    //    }
    //}
}