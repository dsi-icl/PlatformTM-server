using System.Collections.Generic;
using System.Web.Http;
using eTRIKS.Commons.DataAccess.MongoDB;
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/etriksNOSQL")]
    public class eTRIKSNOSQLController : ApiController
    {
        private MongoDbDataRepository _mongoDbService;

        public eTRIKSNOSQLController(MongoDbDataRepository mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }


        //[HttpGet]
        //public MongoDataCollection GetData()
        //{
        //    string queryString = System.Web.HttpContext.Current.Request.Url.Query.ToString();
        //    return _mongoDbService.getNoSQLRecords(queryString);
        //}

        [HttpGet]
        [Route("Distinct")]
        public MongoDataCollection GetDistinctData()
        {
            string queryString = System.Web.HttpContext.Current.Request.Url.Query.ToString();
            return _mongoDbService.getDistinctNoSQLRecords(queryString);
        }

        [HttpDelete]
        public string deleteRecord(MongoDocument record)
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
