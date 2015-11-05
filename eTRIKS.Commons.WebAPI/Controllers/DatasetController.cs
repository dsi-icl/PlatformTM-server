using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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
        private FileService _fileService;

        public DatasetController(DatasetService datasetService, FileService fileService)
        {
            _datasetService = datasetService;
            _fileService = fileService;
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
            datasetDTO.Id = addedDataset.Id;
            if (addedDataset != null)
            {
                var response = Request.CreateResponse<DatasetDTO>(HttpStatusCode.Created, datasetDTO);
                string uri = Url.Link("GetDatasetById", new { datasetId = addedDataset.Id, activityId = datasetDTO.ActivityId });
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

        [HttpPost]
        [Route("api/Datasets/{datasetId}/update")]
        public string updateDatasetPost(string datasetId, [FromBody] DatasetDTO datasetDTO)
        {

            return _datasetService.updateDataset(datasetDTO, datasetId);
        }

        // DELETE: api/Dataset/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("api/Dataset/{datasetId}/templateMap")]
        public DataTemplateMap getDatasetTemplateMap(int datasetId)
        {
            return  _datasetService.GeTemplateMaps(datasetId);
        }

        [HttpPost]
        [Route("api/Datasets/{datasetId}/mapToTemplate")]
        public async Task<bool> MapToTemplate(int datasetId, [FromBody] DataTemplateMap dataTemplateMap)
        {
            //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
            //string filePath = PATH + "\\" + fileName;
            return _datasetService.mapToTemplate(datasetId, dataTemplateMap);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/preview")]
        public async Task<Hashtable> getDatasetPreview(int datasetId)
        {
           return _datasetService.getDatasetPreview(datasetId);
        }

        [HttpGet]
        [Route("api/Datasets/{datasetId}/loadDataFile")]
        public async Task<string> LoadDataSetFromFile(int datasetId)
        {
            _datasetService.loadDataset(datasetId);
            return "";
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/loadObservations")]
        public async Task<bool> loadObservations(int datasetId)
        {
            return await _datasetService.loadObservations(datasetId);
        }

        [HttpGet]
        [Route("api/Datasets/{datasetId}/dataFile/header")]
        public async Task<List<Dictionary<string, string>>> LinkFile(int datasetId)
        {
            //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            //string path = rawFilesDirectory + studyId;
            return _datasetService.getFileColHeaders(datasetId);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/OriFileInfo")]
        public async Task<FileDTO> checkTemplateMatch(int datasetId)
        {
             return _datasetService.getDatasetFileInfo(datasetId);
        }
    }
}
