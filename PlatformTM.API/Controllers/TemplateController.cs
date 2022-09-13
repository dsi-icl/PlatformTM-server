﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;

namespace PlatformTM.API.Controllers
{
    [Route("templates")]
    public class TemplateController : Controller
    {
        private readonly TemplateService _templateService;

        public TemplateController(TemplateService templateService)
        {
            _templateService = templateService;
        }

        [HttpGet("subject")]
        public IEnumerable<DatasetDTO> GetSubjectTemplates()
        {
            return _templateService.GetSubjectTemplates();
        }

        [HttpGet("clinical")]
        public IEnumerable<DatasetDTO> Get()
        {
            return _templateService.GetAllDomainTemplates();
        }

        [HttpGet("clinical/{domainId}")]
        public DatasetDTO GetClinicalTemplates(string domainId)
        {
            return _templateService.GetTemplateDataset(domainId);
        }

        [HttpGet("assay/features/{domainId}")]
        public DatasetDTO GetFeatureTemplate(string domainId)
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
