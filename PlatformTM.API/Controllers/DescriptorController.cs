using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;

namespace PlatformTM.API.Controllers
{
    [Route("descriptors")]
    public class DescriptorController : Controller
    {
        private readonly DatasetDescriptorService _datasetDescriptorService;

        public DescriptorController(DatasetDescriptorService descriptorService)
        {
            _datasetDescriptorService = descriptorService;
        }

        [HttpGet("{datasetId}", Name = "GetDescriptorById")]
        public DatasetDTO GetDatasetDescriptor(int datasetId)
        {
            return _datasetDescriptorService.GetActivityDatasetDTO(datasetId);
        }


        [HttpPost]
        public IActionResult AddDatasetDescriptor([FromBody] DatasetDTO datasetDTO)
        {
            var addedDataset = _datasetDescriptorService.addDataset(datasetDTO);
            if (addedDataset != null)
            {
                return new CreatedAtActionResult("GET", "GetDescriptorById", new { datasetId = addedDataset.Id }, addedDataset);
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
