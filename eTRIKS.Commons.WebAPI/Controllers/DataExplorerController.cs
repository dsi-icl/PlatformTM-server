using System.Collections.Generic;
using eTRIKS.Commons.Service.Services;
using System.Collections;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/apps/explore")]
    [Authorize]
    public class DataExplorerController : Controller
    {
        private DataExplorerService _dataService;

        public DataExplorerController(DataExplorerService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        [Route("projects/{projectId}/subjcharacteristics/browse")]
        public List<ObservationRequestDTO> getSubjectCharacteristics(int projectId)
        {
            return _dataService.GetSubjectCharacteristics(projectId);
        }

        [HttpPost]
        [Route("projects/{projectId}/subjects/search")]
        public  Hashtable GetSubjectData(int projectId, [FromBody] List<ObservationRequestDTO> requestedSCs)
        {
            return  _dataService.GetSubjectData(projectId, requestedSCs);
        }

        [HttpPost]
        [Route("projects/{projectId}/observations/clinical/search")]
        public Hashtable GetObservations(int projectId, [FromBody] List<ObservationRequestDTO> observations)
        {
            return _dataService.GetObservationsData(projectId, observations);
        }

        [HttpPost]
        [Route("projects/{projectId}/observations/clinical/group")]
        public ObservationNode GroupObservations(int projectId, [FromBody] List<ObservationRequestDTO> observations)
        {
            return _dataService.GroupObservations(projectId, observations);
        }

        [HttpGet]
        [Route("projects/{projectId}/observations/clinical/browse")]
        public async Task<IEnumerable<ClinicalDataTreeDTO>> getClinicalTree(int projectId)
        {
            return await _dataService.GetClinicalObsTree(projectId);
        }

    }
}
