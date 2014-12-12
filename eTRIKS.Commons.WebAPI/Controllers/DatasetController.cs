using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DatasetController : ApiController
    {
        private DatasetService _datasetService;

        public DatasetController(DatasetService datasetService)
        {
            _datasetService = datasetService;
        }
        
        // GET: api/Dataset
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        //// GET: api/Dataset/5
        //public DomainDataset Get(string id)
        //{
        //    return _datasetService.GetTemplateDataset(id);
        //}

        //Customisation (Dilshan)

        public DomainDataset GetDomain(string id)
        {
            return _datasetService.GetTemplateDatasetNew(id);
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
