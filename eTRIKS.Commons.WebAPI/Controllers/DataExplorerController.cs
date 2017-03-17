using System;
using System.Collections.Generic;
using eTRIKS.Commons.Service.Services;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.DTOs.Explorer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eTRIKS.Commons.WebAPI.Extensions;
using Microsoft.AspNetCore.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("apps/explore")]
    public class DataExplorerController : Controller
    {
        private readonly DataExplorerService _explorerService;
        private readonly QueryService _queryService;

        public DataExplorerController(DataExplorerService explorerService, QueryService queryService)
        {
            _explorerService = explorerService;
            _queryService = queryService;
        }

        [HttpGet("projects/{projectId}/subjcharacteristics/browse")]
        public IActionResult GetSubjectCharacteristics(int projectId)
        {
            var subjChars = _explorerService.GetSubjectCharacteristics(projectId);
            if (subjChars != null)
                return Ok(subjChars);
            return NotFound();
        }
        
        [HttpPost("projects/{projectId}/saveQuery")]
        public IActionResult SaveQuery(int projectId, [FromBody] CombinedQueryDTO cdto )
       {
          var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
          var savedQuery =  _queryService.SaveQuery(cdto, userId, projectId);
            
            if (savedQuery != null)
                return new CreatedAtRouteResult("GetSavedQuery", new { projectId = projectId, queryId = savedQuery.Id.ToString() }, savedQuery);
            
            return new StatusCodeResult(StatusCodes.Status409Conflict);
       }


        [HttpGet("projects/{projectId}/queries/{queryId}", Name = "GetSavedQuery")]
        public IActionResult GetSavedQuery(int projectId, string queryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            var query = _queryService.GetSavedCombinedQuery(projectId, userId,queryId);
            if(query != null)
                return Ok(query);
            return NotFound();
        }

        [HttpGet("projects/{projectId}/queries/browse", Name = "")]
        public IActionResult GetSavedQueries(int projectId) 
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
            var queries = _queryService.GetSavedQueries(projectId, userId);
            if (queries != null)
                return Ok(queries);
            return NotFound();
        }

        /*
        [Route("projects/{projectId}/UpdateQueries")]
        [HttpGet]
        //public IEnumerable<CombinedQueryDTO> Get()
        public List<CombinedQuery> UpdateQueries(CombinedQueryDTO cdto, int projectId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.Identity.IsAuthenticated)
                return null;
            return _explorerService.UpdateQueries(cdto, projectId, userId);
        }
        */
         
        [HttpPost("projects/{projectId}/subjects/search")]
        public  DataTable GetSubjectData(int projectId, [FromBody] List<ObservationRequestDTO> requestedSCs)
        {
            return  _explorerService.GetSubjectData(projectId, requestedSCs);
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

        [HttpGet("projects/{projectId}/observations/clinical/browse")]
        public async Task<IEnumerable<ClinicalDataTreeDTO>> GetClinicalTree(int projectId)
        {
            return await _explorerService.GetClinicalObsTree(projectId);
        }

        [HttpGet("projects/{projectId}/assays/browse")]
        public List<AssayBrowserDTO> GetAssays(int projectId)
        {
            return _explorerService.GetProjectAssays(projectId);
        }

        [HttpPost("projects/{projectId}/assays/{assayId}/samples/search")]
        public DataTable GetAssaySamples(int projectId, int assayId, [FromBody] List<ObservationRequestDTO> characteristics)
        {
            return _explorerService.GetSampleDataForAssay(assayId, characteristics);
        }
    }
}
