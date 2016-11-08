using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Authorize]
    public class DataVisulaiserController : Controller
    {
        private DataService _dataService;

        public DataVisulaiserController(DataService dataService)
        {
            _dataService = dataService;
        }
        

        [HttpGet]
        [Route("api/visualise/clinicalTree/{projectAccession}")]
        public async Task<IEnumerable<ClinicalDataTreeDTO>> getClinicalTree(string projectAccession)
        {
            return await _dataService.GetClinicalObsTree(projectAccession);
        }
    }
}
