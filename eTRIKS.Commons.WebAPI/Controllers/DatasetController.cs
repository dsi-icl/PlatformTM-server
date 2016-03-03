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
        public string updateDataset(int datasetId, [FromBody] DatasetDTO datasetDTO)
        {
            if (datasetDTO.Id == datasetId)
                return _datasetService.updateDataset(datasetDTO);
            return "FAILED to update datasetId";
        }

        [HttpPost]
        [Route("api/Datasets/{datasetId}/update")]
        public string updateDatasetPost(int datasetId, [FromBody] DatasetDTO datasetDTO)
        {

            if (datasetDTO.Id == datasetId)
                return _datasetService.updateDataset(datasetDTO);
            return "FAILED to update datasetId";
        }

        // DELETE: api/Dataset/5
        public void Delete(int id)
        {
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/templateMap")]
        public DataTemplateMap getDatasetTemplateMap(int datasetId)
        {
            return  _datasetService.GetTemplateMaps(datasetId);
        }

        [HttpPost]
        [Route("api/Datasets/{datasetId}/mapToTemplate/file/{fileId}")]
        public async Task<int?> MapToTemplate(int datasetId, int fileId, [FromBody] DataTemplateMap dataTemplateMap)
        {
            //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
            //string filePath = PATH + "\\" + fileName;
            return _datasetService.mapToTemplate(datasetId,fileId, dataTemplateMap);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/preview/file/{fileId}")]
        public async Task<Hashtable> getDatasetPreview(int datasetId, int fileId)
        {
           return _datasetService.getDatasetPreview(datasetId,fileId);
        }

        [HttpGet]
        [Route("api/Datasets/{datasetId}/saveDataFile/file/{fileId}")]
        public async Task<string> LoadDataFile(int datasetId, int fileId)
        {
            _datasetService.SaveDataFile(datasetId,fileId);
            return "";
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/loadData/file/{fileId}")]
        public async Task<bool> LoadData(int datasetId, int fileId)
        {
            return await _datasetService.LoadDataset(datasetId,fileId);
        }

        //[HttpGet]
        //[Route("api/Datasets/{datasetId}/dataFile/header")]
        //public async Task<List<Dictionary<string, string>>> LinkFile(int datasetId)
        //{
        //    //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
        //    //string path = rawFilesDirectory + studyId;
        //    return _datasetService.getFileColHeaders(datasetId);
        //}

        [HttpGet]
        [Route("api/datasets/{datasetId}/OriFileInfo/{fileId}")]
        public async Task<FileDTO> checkTemplateMatch(int datasetId, int fileId)
        {
             return _datasetService.getDatasetFileInfo(datasetId,fileId);
        }
    }
}
