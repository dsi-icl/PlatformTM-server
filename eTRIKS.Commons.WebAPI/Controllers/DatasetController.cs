using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    public class DatasetController : Controller
    {
        private DatasetService _datasetService;
        private FileService _fileService;

        public DatasetController(DatasetService datasetService, FileService fileService)
        {
            _datasetService = datasetService;
            _fileService = fileService;
        }
        
        [HttpGet]
        [Route("api/templates/clinical")]
        public IEnumerable<DatasetDTO> Get()
        {
            return _datasetService.GetAllDomainTemplates();
        }
        
        [HttpGet]
        [Route("api/templates/clinical/{domainId}")]
        public DatasetDTO Get(string domainId)
        {
            return _datasetService.GetTemplateDataset(domainId);
        }

        [HttpGet]
        [Route("api/templates/assay/features")]
        public List<DatasetDTO> GetAssayFeatureTemplates()
        {
            return _datasetService.GetAssayFeatureTemplates();
        }

        [HttpGet]
        [Route("api/templates/assay/samples")]
        public List<DatasetDTO> GetAssaySampleTemplates()
        {
            return _datasetService.GetAssaySampleTemplates();
        }

        [HttpGet]
        [Route("api/templates/assay/data")]
        public List<DatasetDTO> GetAssayDataTemplates()
        {
            return _datasetService.GetAssayDataTemplates();
        }

        [HttpGet]
        [Route("api/activities/{activityId}/datasets/{datasetId}", Name = "GetDatasetById")]
        public DatasetDTO GetActivityDataset(int datasetId)
        {
            return _datasetService.GetActivityDatasetDTO(datasetId);
        }


        //[HttpPost]
        //[Route("api/Dataset")]
        //public IActionResult addDataset([FromBody] DatasetDTO datasetDTO)
        //{
        //    var addedDataset = _datasetService.addDataset(datasetDTO);
        //    datasetDTO.Id = addedDataset.Id;
        //    if (addedDataset != null)
        //    {
        //        var response = new HttpResponseMessage(HttpStatusCode.Created);
        //        response.Content = datasetDTO;
        //        string uri = Url.Link("GetDatasetById", new { datasetId = addedDataset.Id, activityId = datasetDTO.ActivityId });
        //        response.Headers.Location = new Uri(uri);
        //        return response;
        //    }
        //    else
        //    {
        //        var response = Request.CreateResponse(HttpStatusCode.Conflict);
        //        return response;
        //    }
        //}


        //[HttpPut]
        //[Route("api/Dataset/{datasetId}")]
        //public string updateDataset(int datasetId, [FromBody] DatasetDTO datasetDTO)
        //{
        //    if (datasetDTO.Id == datasetId)
        //        return _datasetService.UpdateDataset(datasetDTO);
        //    return "FAILED to update datasetId";
        //}

        [HttpPost]
        [Route("api/datasets/{datasetId}/update")]
        public string updateDatasetPost(int datasetId, [FromBody] DatasetDTO datasetDTO)
        {

            if (datasetDTO.Id == datasetId)
                return _datasetService.UpdateDataset(datasetDTO);
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
        [Route("api/datasets/{datasetId}/mapToTemplate/file/{fileId}")]
        public async Task<int?> MapToTemplate(int datasetId, int fileId, [FromBody] DataTemplateMap dataTemplateMap)
        {
            //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
            //string filePath = PATH + "\\" + fileName;
            return _datasetService.mapToTemplate(datasetId,fileId, dataTemplateMap);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/preview/file/{fileId}")]
        public Hashtable GetDatasetPreview(int datasetId, int fileId)
        {
           return _fileService.getFilePreview(fileId);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/saveDataFile/file/{fileId}")]
        public bool LoadDataFile(int datasetId, int fileId)
        {
           return  _datasetService.PersistSDTM(datasetId, fileId);            
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/loadData/file/{fileId}")]
        public async Task<bool> LoadData(int datasetId, int fileId)
        {
            return await _datasetService.LoadDataset(datasetId,fileId);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/compute/files/{fileId}")]
        public async Task ComputeFields(int datasetId, int fileId)
        {
            //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            //string path = rawFilesDirectory + studyId;
            await _datasetService.GenerateComputeVars(datasetId);
        }

        [HttpGet]
        [Route("api/datasets/{datasetId}/OriFileInfo/{fileId}")]
        public async Task<FileDTO> checkTemplateMatch(int datasetId, int fileId)
        {
             return _datasetService.getDatasetFileInfo(datasetId,fileId);
        }
    }
}
