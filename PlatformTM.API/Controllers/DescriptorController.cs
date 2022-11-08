using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;
using PlatformTM.Services.DTOs;

namespace PlatformTM.API.Controllers
{
    [Route("descriptors")]
    public class DescriptorController : Controller
    {
        private readonly DatasetDescriptorService _datasetDescriptorService;
        private readonly FileService _fileService;

        public DescriptorController(DatasetDescriptorService descriptorService, FileService fileService)
        {
            _datasetDescriptorService = descriptorService;
            _fileService = fileService;
        }

        [HttpGet("{datasetId}", Name = "GetDescriptorById")]
        public DatasetDTO GetDatasetDescriptor(int datasetId)
        {
            return _datasetDescriptorService.GetActivityDatasetDTO(datasetId);
        }

        [HttpPost("upload/{projectId}")]
        public async Task<IActionResult> UploadFile(int projectId)
        {
            try
            {
                var path = _fileService.GetFullPath(projectId)+"/temp/";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);


                if (Request.ContentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var file = Request.Form.Files[0];
                    if (file.Length <= 0) return BadRequest("File size is zero");
                    using (var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);

                    }
                    var fi = new FileInfo(Path.Combine(path, file.FileName));
                    //_fileService.AddOrUpdateFile(projectId, fi, dirId);
                    return Ok();
                }
                return BadRequest($"Expected a multipart request, but got '{Request.ContentType}'");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("load/{filename}/projects/{projectId}")]
        public DatasetDescriptor LoadDatasetDescriptor(int projectId, string filename)
        {
            return _datasetDescriptorService.GetUploadedDescriptor(projectId, filename);
        }

        [HttpPost]
        public IActionResult AddObsDatasetDescriptor([FromBody] ObservationDatasetDescriptor descriptor, int projectId)
        {
            var addedDescriptor = _datasetDescriptorService.AddDescriptor(descriptor,projectId);
            if (addedDescriptor != null)
            {
                return new CreatedAtActionResult("GET", "GetDescriptorById", new { datasetId = addedDescriptor.Id }, addedDescriptor);
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

    }
}
