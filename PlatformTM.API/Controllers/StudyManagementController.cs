using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlatformTM.API.Controllers
{
    [Route("study-management")]
    public class AssessmentController : Controller
    {
        private readonly AssessmentService _assessmentService;
        private readonly PrimaryDatasetService _primaryDatasetService;
        private readonly StudyService _studyService;

        public AssessmentController(AssessmentService assessmentService, StudyService studyService, PrimaryDatasetService primaryDatasetService)
        {
            _assessmentService = assessmentService;
            _studyService = studyService;
            _primaryDatasetService = primaryDatasetService;
        }


        /***
         * Study CRUD
         */

        [HttpGet("studies/{studyId}", Name = "GetstudyById")]
        public StudyDTO Getstudy(int studyId)
        {
            return _studyService.GetstudyId(studyId);
        }

        [HttpPost("studies")]
        public IActionResult Addstudy([FromBody] StudyDTO studyDTO)
        {
            var addedstudy = _studyService.Addstudy(studyDTO);

            if (addedstudy != null)
                return new CreatedAtActionResult("GET", "GetstudyById", new { studyId = addedstudy.Id }, studyDTO);

            return new BadRequestResult();
        }

        [HttpPut("studies/{studyId}")]
        public IActionResult Updatestudy(int studyId, [FromBody] StudyDTO studyDTO)
        {
            try
            {
                _studyService.Updatestudy(studyDTO, studyId);
                return new CreatedAtActionResult("GET", "GetstudyById", new { studyId = studyDTO.Id }, studyDTO);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }



        /**
         * Study Assessments
         */

        [HttpGet("studies/{studyId}/assessments", Name = "GetStudyAssessments")]
        public List<AssessmentDTO> GetStudyAssessment(int studyId)
        {
            return _assessmentService.GetStudyAssessments(studyId);
        }

        [HttpGet("studies/{studyId}/assessments/{assessmentId}", Name = "GetStudyAssessmentById")]
        public AssessmentDTO GetStudyAssessmentById(int assessmentId)
        {
            return _assessmentService.GetAssessmentForStudy(assessmentId);
        }

        [HttpPost("studies/{studyId}/assessments")]
        public IActionResult AddAssessment([FromBody] AssessmentDTO AssessmentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var addedAssessment = _assessmentService.AddStudyAssessment(AssessmentDTO);
            if (addedAssessment != null)
                return new CreatedAtRouteResult("GetStudyAssessmentById", new { assessmentId = addedAssessment.Id }, addedAssessment);
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("studies/{studyId}/assessments")]
        public IActionResult UpdateAssay(int assessmentId, [FromBody] AssessmentDTO AssessmentDTO)
        {
            try
            {
                _assessmentService.UpdateStudyAssessment(AssessmentDTO, assessmentId);
                return new AcceptedAtRouteResult("GetStudyAssessmentById", new { assessmentId = AssessmentDTO.Id }, AssessmentDTO);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        /**
         * Study Datasets
         */

        [HttpGet("studies/{studyId}/datasets", Name = "GetStudyDatasets")]
        public List<PrimaryDatasetDTO> GetStudyDatasetsById(int studyId)
        {
            return _primaryDatasetService.GetPrimaryDatasetsForStudy(studyId);
        }

        [HttpPut("studies/{studyId}/datasets")]
        public IActionResult UpdateStudyDataset([FromBody] AssessmentDTO AssessmentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var addedAssessment = _assessmentService.AddStudyAssessment(AssessmentDTO);
            if (addedAssessment != null)
                return new CreatedAtRouteResult("GetStudyAssessmentById", new { assessmentId = addedAssessment.Id }, addedAssessment);
            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("studies/{studyId}/assessments")]
        public IActionResult UpdateAssay(int studyId, [FromBody] StudyDTO studyDTO)
        {
            try
            {
                _assessmentService.UpdateStudyDatasets(studyDTO, studyId);
                return new AcceptedAtRouteResult("GetStudyById", new { studyId = studyId }, studyDTO);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

    }
}

