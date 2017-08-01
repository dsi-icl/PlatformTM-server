using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("curation")]
    public class CurationController : Controller
    {
       private readonly CurationService _curationService; 

        public CurationController(CurationService curationService)
        {
           _curationService = curationService;
        }
        
        [HttpGet("{fileId}/CsvToSingleColumns")]
        [AllowAnonymous]
        public bool CsvToSingleColumns(int fileId)
        {
            _curationService.CsvToSingleColumns(fileId);
            return true;
        }
    }
} 
