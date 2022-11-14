using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Models.Services;
using PlatformTM.Services.LoadingWizard;
using PlatformTM.Services.LoadingWizard.DTO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlatformTM.API.Controllers
{
    [Route("data-loading")]
    public class LoadingWizardController : Controller
    {

        private readonly LoadingWizardService _loadingWizardService;
        

        public LoadingWizardController(LoadingWizardService loadingService)
        {
            _loadingWizardService = loadingService;
        }

        [HttpGet("projects/{projectId}/study-assessments/")]
        public List<StudyDatasetsDTO> Getstudy(int projectId)
        {
            return _loadingWizardService.GetProjectAssessments(projectId);
        }

        [HttpGet]
        [Route("files/{fileId}/initload")]
        public bool InitLoadFile(int fileId)
        {
            return _loadingWizardService.InitLoading(fileId);
            //if (success)
            //    return Ok(true);
            //return new BadRequestResult();
        }

        [HttpGet]
        [Route("files/{fileId}/load/datasets/{datasetId}/assessments/{assessmentId}")]
        public void LoadFile(int fileId, int datasetId, int assessmentId)
        {
             _loadingWizardService.LoadFile(fileId, datasetId, assessmentId);
            //if (success)
            //    return Ok(true);
            //return new BadRequestResult();
        }

        //[HttpGet]
        //[Route("files/{fileId}/progress")]
        //public IActionResult GetLoadingProgress(int fileId)
        //{
        //    //var progressDTO = _fileService.GetFile(fileId);
        //    //return new OkObjectResult(progressDTO);
        //}

        //[HttpGet]
        //[Route("files/{fileId}/unload")]
        //public async void UnloadFile(int fileId)
        //{
        //    //await _datasetService.UnloadFileDatasets(fileId);
        //}

        //[HttpPost]
        //[Route("{fileId}/mapToTemplate/datasets/{datasetId}")]
        //public int? MapToTemplate(int datasetId, int fileId, [FromBody] DataTemplateMap dataTemplateMap)
        //{
        //    //return _fileService.mapToTemplate(datasetId, fileId, dataTemplateMap);
        //}

        [HttpGet("{fileId}/match/datasets/{datasetId}")]
        public void CheckValidTemplate(int datasetId, int fileId)
        {
            //return _fileService.MatchFileToTemplate(datasetId, fileId);
        }
    }
}

