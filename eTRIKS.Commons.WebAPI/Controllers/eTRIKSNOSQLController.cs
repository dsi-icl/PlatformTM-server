using System.Collections.Generic;
using System.Web.Http;
using eTRIKS.Commons.DataParser.MongoDBAccess;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/etriksNOSQL")]
    public class eTRIKSNOSQLController : ApiController
    {
        private MongoDbDataServices _mongoDbService;

        public eTRIKSNOSQLController(MongoDbDataServices mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        //[Route("")]
        // GET api/values
        //public IEnumerable<string> getRecordsOfStudy()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //[Route("{id}")]
        // GET api/values/5
        [HttpGet]
        public List<NoSQLRecord> GetData()
        {
            string queryString = System.Web.HttpContext.Current.Request.Url.Query.ToString();
            return _mongoDbService.getNoSQLRecord(queryString);
        }

        [HttpDelete]
        public string deleteRecord(NoSQLRecord record)
        {
            return _mongoDbService.deleteDataGeneric(record);
        }

        [HttpPost]
        public string updateRecord(string OID, NoSQLRecordForUpdate updateDetails)
        {
            return _mongoDbService.updateDataGeneric(updateDetails);
        }
    }
}
