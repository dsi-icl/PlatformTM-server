using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net;
using Microsoft.AspNetCore.Http;
using eTRIKS.Commons.Service.Services.Loading.HdDataLoader;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("datasets")]
    public class DatasetController : Controller
    {
        private readonly DatasetService _datasetService;
        private readonly FileService _fileService;
        private readonly DataMatrixLoader _dataMatrixLoader;
        public DatasetController(DatasetService datasetService, FileService fileService, DataMatrixLoader dataMatrixLoader) 
        {

            _datasetService = datasetService;
            _fileService = fileService;
            _dataMatrixLoader = dataMatrixLoader;
        }


        [HttpGet("{datasetId}", Name = "GetDatasetById")]
        public DatasetDTO GetActivityDataset(int datasetId)
        {
            return _datasetService.GetActivityDatasetDTO(datasetId);
        }


        [HttpPost]
        public IActionResult AddDataset([FromBody] DatasetDTO datasetDTO)
        {
            var addedDataset = _datasetService.addDataset(datasetDTO);
            datasetDTO.Id = addedDataset.Id;
            if (addedDataset != null)
            {
                //var response = new HttpResponseMessage(HttpStatusCode.Created);
                //response.Content = datasetDTO;
                //string uri = Url.Link("GetDatasetById", new { datasetId = addedDataset.Id, activityId = datasetDTO.ActivityId });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetDatasetById", new { datasetId = addedDataset.Id }, addedDataset);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }


        //[HttpPut]
        //[Route("api/Dataset/{datasetId}")]
        //public string updateDataset(int datasetId, [FromBody] DatasetDTO datasetDTO)
        //{
        //    if (datasetDTO.Id == datasetId)
        //        return _datasetService.UpdateDataset(datasetDTO);
        //    return "FAILED to update datasetId";
        //}

        [HttpPost("{datasetId}/update")]
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

        //[HttpGet]
        //[Route("api/datasets/{datasetId}/templateMap")]
        public DataTemplateMap getDatasetTemplateMap(int datasetId)
        {
            return  _datasetService.GetTemplateMaps(datasetId);
        }

        //[HttpPost]
        //[Route("api/datasets/{datasetId}/mapToTemplate/file/{fileId}")]
        public int? MapToTemplate(int datasetId, int fileId, [FromBody] DataTemplateMap dataTemplateMap)
        {
            //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");
            //string filePath = PATH + "\\" + fileName;
            return _datasetService.mapToTemplate(datasetId,fileId, dataTemplateMap);
        }

        [HttpGet("{datasetId}/preview/file/{fileId}")]
        public Hashtable GetDatasetPreview(int datasetId, int fileId)
        {
           return _fileService.getFilePreview(fileId);
        }

        [HttpGet("{datasetId}/saveDataFile/file/{fileId}")]
        public bool LoadDataFile(int datasetId, int fileId)
        {
           return  _datasetService.PersistSDTM(datasetId, fileId);            
        }

        [HttpGet("{datasetId}/loadData/file/{fileId}")]
        public async Task<bool> LoadData(int datasetId, int fileId)
        {
            return await _datasetService.LoadDataset(datasetId,fileId);
        }

        [HttpGet("{datasetId}/unloadData/file/{fileId}")]
        public IActionResult UnloadData(int datasetId, int fileId)
        {
            _datasetService.UnloadDataset(datasetId,fileId);
            return Ok();
        }

        //[HttpGet]
        //[Route("api/datasets/{datasetId}/compute/files/{fileId}")]
        public async Task ComputeFields(int datasetId, int fileId)
        {
            //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            //string path = rawFilesDirectory + studyId;
            await _datasetService.GenerateComputeVars(datasetId);
        }

        [HttpGet("{datasetId}/validate/{fileId}")]
        public FileDTO CheckValidTemplate(int datasetId, int fileId)
        {
             return _datasetService.CheckFileTemplateMatch(datasetId,fileId);
        }


        [HttpGet("{datasetId}/loadHDdDdata/{fileId}")]
        public bool LoadHDdDdata(int datasetId, int fileId/*, int referencFromHdId*/ )
        {
             return  _dataMatrixLoader.LoadHDdDdata(datasetId, fileId/*, referencFromHdId*/);
        }
         
    } 
}
