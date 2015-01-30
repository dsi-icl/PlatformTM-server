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


        [HttpGet]
        public NoSQLRecordSet GetData()
        {
            string queryString = System.Web.HttpContext.Current.Request.Url.Query.ToString();
            return _mongoDbService.getNoSQLRecords(queryString);
        }

        [HttpGet]
        [Route("Distinct")]
        public NoSQLRecordSet GetDistinctData()
        {
            string queryString = System.Web.HttpContext.Current.Request.Url.Query.ToString();
            return _mongoDbService.getDistinctNoSQLRecords(queryString);
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
