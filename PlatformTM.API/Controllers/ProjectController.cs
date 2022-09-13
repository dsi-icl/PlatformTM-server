﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.DTOs.Explorer;
using PlatformTM.Models.Services;
using PlatformTM.Models.Services.UserManagement;

namespace PlatformTM.API.Controllers
{
    [Route("projects")]
    public class ProjectController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly UserAccountService _accountService;


        public ProjectController(ProjectService projectService, UserAccountService accountService)
        {
            _projectService = projectService;
            _accountService = accountService;
        }

        [HttpGet]
        public IEnumerable<ProjectDTO> Get()
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            return !User.Identity.IsAuthenticated ? null :  _projectService.GetProjects(userId);
        }

        [HttpGet("accession/{projectId}", Name = "GetProjectByAcc")]
        public ProjectDTO GetProjectFull(int projectId)
        {
            return _projectService.GetProjectFullDetails(projectId);
        }

        [HttpGet("{projectId}", Name = "GetProjectById")]
        public ProjectDTO GetProject(int projectId)
        {
            return _projectService.GetProjectById(projectId);
        }

        [HttpGet("{projectId}/allactivities")]
        public IEnumerable<ActivityDTO> GetProjectActivities(int projectId)
        {
            return _projectService.GetActivities(projectId, null);
        }

        [HttpGet("{projectId}/assays")]
        public IEnumerable<ActivityDTO> GetProjectAssays(int projectId)
        {
            return _projectService.GetActivities(projectId,typeof(Assay));
        }

        [HttpGet("{projectId}/subjDataCollection")]
        public IEnumerable<ActivityDTO> GetProjectSubjActivities(int projectId)
        {
            return _projectService.GetActivities(projectId, typeof(SubjectRecording));
        }

        [HttpGet("{projectId}/clinicalAssessments")]
        public IEnumerable<ActivityDTO> GetProjectClinicalAssessments(int projectId)
        {
            return _projectService.GetActivities(projectId, typeof(Activity));
        }

        [HttpGet("{projectId}/datasets/clinical")]
        [AllowAnonymous]
        public IActionResult GetProjectClinicalDatasets(int projectId)
        {
            //var userId = User.FindFirst(ClaimTypes.UserData).Value;
            //if (!User.Identity.IsAuthenticated)
                //return new UnauthorizedResult();
            var result = _projectService.GetProjectClinicalDatasets(projectId);
            return new OkObjectResult(result);
        }

        [HttpGet("{projectId}/datasets/assays")]
        [AllowAnonymous]
        public IActionResult GetProjectAssayDatasets(int projectId)
        {
            //var userId = User.FindFirst(ClaimTypes.UserData).Value;
            //if (!User.Identity.IsAuthenticated)
                //return new UnauthorizedResult();
            var result = _projectService.GetProjectAssayDatasets(projectId);
            return new OkObjectResult(result);
        }


        [HttpGet("{projectId}/users")]
        public IActionResult GetProjectUsers(int projectId)
        {
            var users = _projectService.GetProjectUsers(projectId);
            if (users != null)
                return new OkObjectResult(users);
            return NotFound();
        }

        [HttpGet]
        [Route("{projectId}/remove")]
        public void DeleteProject(int projectId) 
        {
            _projectService.DeleteProject(projectId);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddProject([FromBody] ProjectDTO projectDTO)
        {
            ProjectDTO addedProject = null;

           
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            addedProject = _projectService.AddProject(projectDTO,userId);
            if (addedProject == null)
                return new StatusCodeResult(StatusCodes.Status409Conflict);

            var accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var res = await _accountService.AddUserRole("all", projectDTO.Id, accountId);

            if (addedProject != null && res.Succeeded)
                return new CreatedAtRouteResult("GetProjectByAcc", new { projectId = addedProject.Id }, addedProject);
   
            return new StatusCodeResult(StatusCodes.Status409Conflict);
            
        }

        [HttpPut("{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] ProjectDTO projectDTO)
        {
            try
            {
                _projectService.UpdateProject(projectDTO, projectId);
                return new CreatedAtRouteResult("GetProjectById", new { projectId = projectDTO.Id }, projectDTO);

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
