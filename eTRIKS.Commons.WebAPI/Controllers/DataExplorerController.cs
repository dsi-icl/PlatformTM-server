using System.Collections.Generic;
using eTRIKS.Commons.Service.Services;
using System.Collections;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [Route("projects/{projectId}/SaveDataCart")]
        public void SaveDataCart([FromBody] List<ObservationRequestDTO> oRequests, System.Guid userId)

        {

            _explorerService.SaveDataCart(oRequests, userId);

        }
    


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
