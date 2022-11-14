using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.DTOs.Explorer;
using PlatformTM.Models.Services;
using PlatformTM.Services.DatasetExplorer;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlatformTM.API.Controllers
{
    [Route("dataset-explorer")]
    public class PrimaryDatasetsController : Controller
    {
        private readonly DatasetExplorerService _datasetExplorerService;
        private readonly PrimaryDatasetService _primaryDatasetService;
        private readonly StudyService _studyService;

        public PrimaryDatasetsController(DatasetExplorerService explorerService)
        {
            _datasetExplorerService = explorerService;
   
        }
    

        
        /**
          * Project Primary Datasets
          */

        [HttpGet("datasets/{datasetId}")]
        public async Task<ClinicalExplorerDTO> GetClinicalTreeAsync(int datasetId)
        {
            return await _datasetExplorerService.GetClinicalObsTree(datasetId);
        }

        [HttpGet("datasets/{datasetId}/features")]
        public  List<Feature> GetDatasetFeatures(int datasetId)
        {
            return  _datasetExplorerService.GetObservationPhenos(datasetId);
        }

        //[HttpGet("projects/{projectId}/datasets/{datasetId}", Name = "GetProjectDatasetById")]
        //public PrimaryDatasetDTO GetProjectDatasetById(int datasetId)
        //{
        //    return _primaryDatasetService.GetPrimaryDatasetInfo(datasetId);
        //}

        //[HttpPost("projects/{projectId}/datasets")]
        //public IActionResult AddDataset([FromBody] PrimaryDatasetDTO primaryDatasetDTO)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest();
        //    var addedDataset = _primaryDatasetService.AddPrimaryDatasetInfo(primaryDatasetDTO);
        //    if (addedDataset != null)
        //        return new CreatedAtRouteResult("GetProjectDatasetById", new { datasetId = addedDataset.Id }, addedDataset);
        //    return new StatusCodeResult(StatusCodes.Status409Conflict);
        //}

        //[HttpPut("projects/{projectId}/datasets")]
        //public IActionResult UpdateAssay(int projectId, [FromBody] PrimaryDatasetDTO primaryDatasetDTO)
        //{
        //    try
        //    {
        //        _primaryDatasetService.UpdatePrimaryDatasetInfo(primaryDatasetDTO);
        //        return new AcceptedAtRouteResult("GetProjectDatasetById", new { datasetId = primaryDatasetDTO.Id }, primaryDatasetDTO);
        //    }
        //    catch (Exception e)
        //    {
        //        return new BadRequestObjectResult(e.Message);
        //    }
        //}
    }
}

