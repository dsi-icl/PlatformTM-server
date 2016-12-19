using System.Collections.Generic;
using eTRIKS.Commons.Service.Services;
using System.Collections;
using System.Security.Claims;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using eTRIKS.Commons.WebAPI.Extensions;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("apps/explore")]
    public class DataExplorerController : Controller
    {
        private readonly DataExplorerService _explorerService;

        public DataExplorerController(DataExplorerService explorerService)
        {
            _explorerService = explorerService;
        }

        [HttpGet("projects/{projectId}/subjcharacteristics/browse")]
        public List<ObservationRequestDTO> getSubjectCharacteristics(int projectId)
        {
            return _explorerService.GetSubjectCharacteristics(projectId);
        }



        [HttpPost]
        [Route("projects/{projectId}/SaveQueries")]
        public void SaveQueries(CombinedQueryDTO cdto, int projectId)
       {
         var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //retrieve user combined queries we will directly query mongo using userId we get from the controller
            _explorerService.SaveQueries(cdto, userId, projectId);

        }

        [Route("projects/{projectId}/GetSavedQueries")]
        [HttpGet]
        //public IEnumerable<CombinedQueryDTO> Get()
        public List<CombinedQuery> GetSavedQueries(int projectId) 
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (!User.Identity.IsAuthenticated)
                return null;
            return _explorerService.GetSavedQueries(projectId, userId);
        }

        //[Route("projects/{projectId}/UpdateQueries")]
        //[HttpGet]
        ////public IEnumerable<CombinedQueryDTO> Get()
        //public List<CombinedQuery> UpdateQueries(CombinedQueryDTO cdto, int projectId)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        //    if (!User.Identity.IsAuthenticated)
        //        return null;
        //    return _explorerService.UpdateQueries(cdto, projectId, userId);
        //}

        [HttpPost("projects/{projectId}/subjects/search")]
        public  Hashtable GetSubjectData(int projectId, [FromBody] List<ObservationRequestDTO> requestedSCs)
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
        public List<AssayDTO> GetAssays(int projectId)
        {
            return _explorerService.GetProjectAssays(projectId);
        }

    }
}
