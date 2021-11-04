using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services;
using PlatformTM.Services.Services.HelperService;
using PlatformTM.Services.Services.Loading.AssayData;

namespace PlatformTM.API.Controllers
{
    [Route("datasets")]
    public class DatasetController : Controller
    {
        private readonly DatasetDescriptorService _datasetDescriptorService;
        private readonly DatasetService _datasetService;
        private readonly DataMatrixLoader _dataMatrixLoader;
        private readonly FileService _fileService;

        public DatasetController(DatasetDescriptorService descriptorService,
                                 DatasetService datasetservice,
                                 FileService fileService,
                                 DataMatrixLoader dataMatrixLoader) 
        {
            _datasetDescriptorService = descriptorService;
            _datasetService = datasetservice;
            _dataMatrixLoader = dataMatrixLoader;
            _fileService = fileService;
        }


        [HttpGet("{datasetId}", Name = "GetDatasetById")]
        public DatasetDTO GetActivityDataset(int datasetId)
        {
            return _datasetDescriptorService.GetActivityDatasetDTO(datasetId);
        }


        [HttpPost]
        public IActionResult AddDataset([FromBody] DatasetDTO datasetDTO)
        {
            var addedDataset = _datasetDescriptorService.addDataset(datasetDTO);
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
                return _datasetDescriptorService.UpdateDataset(datasetDTO);
            return "FAILED to update datasetId";
        }
        
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

        [HttpGet("{datasetId}/descriptor")]
        [AllowAnonymous]
        public DatasetDTO DownloadDatasetDescriptor(int datasetId)
        {
            var json= _datasetDescriptorService.GetActivityDatasetDTO(datasetId);
            //var datatable = await _datasetService.ConsolidateDataset(datasetId);
            return json;
            //byte[] outputBuffer = null;

            //using (MemoryStream tempStream = new MemoryStream())
            //{
            //    using (StreamWriter writer = new StreamWriter(tempStream))
            //    {
            //        JsonSerializer serializer = new JsonSerializer();
            //        //serialize object directly into file stream
            //        serializer.Serialize(writer, json);
            //        //IOhelper.WriteDataTable(datatable, writer, true);
            //    }

            //    outputBuffer = tempStream.ToArray();
            //}
            ////Response.Headers.Add("Content-Disposition", "inline; filename="+datatable.TableName + ".csv");
            //return File(outputBuffer, "application/json", json.Name + ".json");

        }

        [HttpGet("{datasetId}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadDataset(int datasetId){
            var datatable = await _datasetService.ConsolidateDataset(datasetId);

            byte[] outputBuffer = null;

            using (MemoryStream tempStream = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(tempStream))
                {
                    IOhelper.WriteDataTable(datatable, writer, true);
                }

                outputBuffer = tempStream.ToArray();
            }
            //Response.Headers.Add("Content-Disposition", "inline; filename="+datatable.TableName + ".csv");
            return File(outputBuffer, "text/csv", datatable.TableName+".csv");

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
