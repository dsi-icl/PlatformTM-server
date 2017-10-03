using System;
using System.Collections.Generic;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.v3.RemoteRepositories;

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

        // save the csv file to single Rows
        [HttpGet("{fileId}/CsvToSingleRows")]
        [AllowAnonymous]
        public bool CsvToSingleRows(int fileId)
        {
            _curationService.CsvToSingleRows(fileId);
            return true;
        }

        // save the csv file to single columns
        [HttpGet("{fileId}/CsvToSingleColumns")]
        [AllowAnonymous]
        public bool CsvToSingleColumns(int fileId)
        {
            _curationService.CsvToSingleRows(fileId);
            return true;
        }

        // this method is called by front end for each header name and get the suggestions 
        [HttpGet("{fileId}/GetSuggestions")]
        [AllowAnonymous]
        public List<List<CurationDTO>> CurationSuggestions(int fileId)
        {
            return _curationService.GetSuggestions(fileId);

        }
    }
} 
