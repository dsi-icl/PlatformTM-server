using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Core.Domain.Model.Templates;

using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.DTOs;

using System.Web.Http.Cors;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [EnableCors(origins: "http://localhost:63342", headers: "*", methods: "GET,POST")]
    public class DatasetController : ApiController
    {
        private DatasetService _datasetService;

        public DatasetController(DatasetService datasetService)
        {
            _datasetService = datasetService;
        }
        
        // GET: api/Dataset
        //[EnableCors(origins: "http://localhost:63342", headers: "*", methods: "*")]
        [HttpGet]
        [Route("api/Dataset")]
        public IEnumerable<DomainTemplate> Get()
        {
            //List<DomainTemplate> ts = new List<DomainTemplate>();
            //DomainTemplate dt = new DomainTemplate();
            //dt.Class = "test1";
            //ts.Add(dt);
            //dt = new DomainTemplate();
            //dt.Class = "test2";
            //ts.Add(dt);
            //return ts;
            return _datasetService.GetAllDomainTemplates();
        }

       

        // GET: api/Dataset/5
        //[EnableCors(origins: "http://localhost:63342", headers: "*", methods: "*")]
        [HttpGet]
        [Route("api/Dataset/{domainId}")]
        public DatasetDTO Get(string domainId)
        {
            return _datasetService.GetTemplateDataset(domainId);
        }

        // POST: api/Dataset
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Dataset/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Dataset/5
        public void Delete(int id)
        {
        }
    }
}
