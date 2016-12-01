using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("api/templates")]
    public class TemplateController : Controller
    {
        private readonly TemplateService _templateService;

        public TemplateController(TemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet("clinical")]
        public IEnumerable<DatasetDTO> Get()
        {
            return _templateService.GetAllDomainTemplates();
        }

        [HttpGet("clinical/{domainId}")]
        public DatasetDTO Get(string domainId)
        {
            return _templateService.GetTemplateDataset(domainId);
        }

        [HttpGet("assay/features")]
        public List<DatasetDTO> GetAssayFeatureTemplates()
        {
            return _templateService.GetAssayFeatureTemplates();
        }

        [HttpGet("assay/samples")]
        public List<DatasetDTO> GetAssaySampleTemplates()
        {
            return _templateService.GetAssaySampleTemplates();
        }

        [HttpGet("assay/data")]
        public List<DatasetDTO> GetAssayDataTemplates()
        {
            return _templateService.GetAssayDataTemplates();
        }
    }
}
