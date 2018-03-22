using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services;
using PlatformTM.Services.Services.Loading.AssayData;

namespace PlatformTM.API.Controllers
{
    [Route("datasets")]
    public class DatasetController : Controller
    {
        private readonly DatasetService _datasetService;
        private readonly DataMatrixLoader _dataMatrixLoader;

        public DatasetController(DatasetService datasetService, DataMatrixLoader dataMatrixLoader) 
        {
            _datasetService = datasetService;
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
            if (addedDataset != null)
            {
                return new CreatedAtActionResult("GET", "GetDatasetById", new { datasetId = addedDataset.Id }, addedDataset);
            }
            return new StatusCodeResult(StatusCodes.Status409Conflict);
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
        public string UpdateDatasetPost(int datasetId, [FromBody] DatasetDTO datasetDTO)
        {
            if (datasetDTO.Id == datasetId)
                return _datasetService.UpdateDataset(datasetDTO);
            return "FAILED to update datasetId";
        }

        //[HttpGet]
        //[Route("{datasetId}/templateMap")]
        //public DataTemplateMap GetDatasetTemplateMap(int datasetId)
        //{
        //    return  _datasetService.GetTemplateMaps(datasetId);
        //}

        
        [HttpGet("{datasetId}/load/files/{fileId}")]
        public async Task<bool> LoadDataset(int datasetId, int fileId)
        {
            return await _datasetService.LoadDataset(datasetId,fileId);
        }

        [HttpGet("{datasetId}/unload/files/{fileId}")]
        public IActionResult UnloadDataset(int datasetId, int fileId)
        {
            _datasetService.UnloadDataset(datasetId,fileId);
            return Ok();
        }

        //[HttpGet]
        //[Route("api/datasets/{datasetId}/compute/files/{fileId}")]
        //public async Task ComputeFields(int datasetId, int fileId)
        //{
        //    //string rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
        //    //string path = rawFilesDirectory + studyId;
        //    await _datasetService.GenerateComputeVars(datasetId);
        //}

        [HttpGet("{datasetId}/loadHDdDdata/{fileId}")]
        public bool LoadHDdDdata(int datasetId, int fileId/*, int referencFromHdId*/ )
        {
             return  _dataMatrixLoader.LoadHDdDdata(datasetId, fileId/*, referencFromHdId*/);
        }
         
    } 
}
