using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.DTOs.Explorer;
using PlatformTM.Services.Services;

namespace PlatformTM.API.Controllers
{
    [Route("apps/explore")]
    public class DataExplorerController : Controller
    {
        private readonly DataExplorerService _explorerService;
        private readonly QueryService _queryService;
        private readonly ProjectService _projectService;

        public DataExplorerController(DataExplorerService explorerService, QueryService queryService, ProjectService projectService)
        {
            _explorerService = explorerService;
            _queryService = queryService;
            _projectService = projectService;
        }

        [HttpGet("projects/{projectId}/assaysinit")]
        public async Task<IActionResult> GetAssayDataAsync(int projectId){
            var data = await _explorerService.GetInitAssayData(projectId);
            if (data != null)
                return Ok(data);
            return NotFound();
        }

        [HttpGet("projects/{projectId}/subjectsinit")]
        public async Task<IActionResult> GetSubjDataAsync(int projectId)
        {
            var data = await _explorerService.GetInitSubjectData(projectId);
            if (data != null)
                return Ok(data);
            return NotFound();
        }
        //[HttpGet("projects/{projectId}/subjcharacteristics/browse")]
        //public async Task<IActionResult> GetSubjectCharacteristicsAsync(int projectId)
        //{
        //    var subjChars = await _explorerService.GetSubjectCharacteristics(projectId);
        //    if (subjChars != null)
        //        return Ok(subjChars);
        //    return NotFound();
        //}

         
        [HttpPost("projects/{projectId}/subjects/search")]
        public  async Task<IActionResult> GetSubjectDataAsync(int projectId, [FromBody] List<ObservationRequestDTO> requestedSCs)
        {
            var data = await _explorerService.GetSubjectData(projectId, requestedSCs);
            if (data != null)
                return Ok(data);
            return NotFound();
        }

        [HttpPost("projects/{projectId}/observations/clinical/search")]
        public Hashtable GetObservations(int projectId, [FromBody] List<ObservationRequestDTO> observations)
        {
            return _explorerService.GetObservationsData(projectId, observations);
        }

        [HttpPost("projects/{projectId}/observations/clinical/group")]
        public ObservationNode GroupObservations(int projectId, [FromBody] List<ObservationRequestDTO> observations)
        {
            return _explorerService.GroupObservations(projectId, observations);
        }
        [HttpPost("projects/{projectId}/observations/clinical/{obsId}/qualifiers")]
        public List<ObservationRequestDTO> GetObservationQualifiers(int projectId, [FromBody] ObservationRequestDTO obsReq)
        {
            return _explorerService.GetObsQualifierRequests(projectId, obsReq);
        }

        [HttpGet("projects/{projectId}/observations/clinical/browse")]
        public async Task<ClinicalExplorerDTO> GetClinicalTreeAsync(int projectId)
        {
            return await _explorerService.GetClinicalObsTree(projectId);
        }

        [HttpGet("projects/{projectId}/assays/browse")]
        public async Task<List<AssayBrowserDTO>> GetAssaysAsync(int projectId)
        {
            return await _explorerService.GetProjectAssays(projectId);
        }

        [HttpPost("projects/{projectId}/assays/{assayId}/samples/search")]
        public async Task<Hashtable> GetAssaySamplesAsync(int projectId, int assayId, [FromBody] List<ObservationRequestDTO> characteristics)
        {
            return await _explorerService.GetSampleDataForAssay(assayId, characteristics);
        }
    }
}
