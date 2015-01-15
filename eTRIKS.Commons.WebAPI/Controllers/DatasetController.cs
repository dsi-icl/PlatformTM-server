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
using eTRIKS.Commons.Core.Domain.Model;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [EnableCors(origins: "http://localhost:63342", headers: "*", methods: "*")] 
    public class DatasetController : ApiController
    {
        private DatasetService _datasetService;

        public DatasetController(DatasetService datasetService)
        {
            _datasetService = datasetService;
        }
        
        // GET: api/Dataset
        [HttpGet]
        [Route("api/Dataset")]
        public IEnumerable<DatasetDTO> Get()
        {
            return _datasetService.GetAllDomainTemplates();
        }

      
        // GET: api/Dataset/5
        [HttpGet]
        [Route("api/Dataset/{domainId}")]
        public DatasetDTO Get(string domainId)
        {
            return _datasetService.GetTemplateDataset(domainId);
        }

        [HttpGet]
        [Route("api/activities/{activityId}/datasets/{datasetId}", Name = "GetDatasetById")]
        public DatasetDTO GetActivityDataset(int datasetId)
        {
            return _datasetService.GetActivityDatasetDTO(datasetId);
        }


        [HttpPost]
        [Route("api/Dataset")]
        public HttpResponseMessage addDataset([FromBody] DatasetDTO datasetDTO)
        {
            var addedDataset = _datasetService.addDataset(datasetDTO);
            datasetDTO.Id = addedDataset.OID;
            if (addedDataset != null)
            {
                var response = Request.CreateResponse<DatasetDTO>(HttpStatusCode.Created, datasetDTO);
                string uri = Url.Link("GetDatasetById", new { datasetId = addedDataset.OID, activityId = datasetDTO.ActivityId });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.Conflict);
                return response;
            }
        }


        [HttpPut]
        [Route("api/Dataset/{datasetId}")]
        public string updateDataset(string datasetId, [FromBody] DatasetDTO datasetDTO)
        {

            return _datasetService.updateDataset(datasetDTO, datasetId);
        }

        // DELETE: api/Dataset/5
        public void Delete(int id)
        {
        }
    }
}
