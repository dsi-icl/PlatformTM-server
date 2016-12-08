using System.Collections.Generic;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;

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
