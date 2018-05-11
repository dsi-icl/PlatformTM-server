using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.DTOs.Explorer;
using PlatformTM.Services.Services;
using PlatformTM.Services.Services.UserManagement;

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

        [HttpGet("{projectId}/activities")]
        public IEnumerable<ActivityDTO> GetProjectActivities(int projectId)
        {
            return _projectService.GetProjectActivities(projectId);
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

        [HttpGet("{projectId}/queries")]
        public List<CombinedQueryDTO> GetUserSavedQueries(int projectId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            if (!User.Identity.IsAuthenticated)
                return null;
            return _projectService.GetProjectSavedQueries(projectId, userId);

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
        public IActionResult AddProject([FromBody] ProjectDTO projectDTO)
        {
            ProjectDTO addedProject = null;

           
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            addedProject = _projectService.AddProject(projectDTO,userId);

            if (addedProject != null)
                return new CreatedAtRouteResult("GetProjectByAcc", new { projectId = addedProject.Id }, addedProject);
   
            return new StatusCodeResult(StatusCodes.Status409Conflict);
            
        }

        [HttpPut("{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] ProjectDTO projectDTO)
        {
            try
            {
                _projectService.UpdateProject(projectDTO, projectId);
                return new CreatedAtActionResult("GET", "GetProjectById", new { projectId = projectDTO.Id }, projectDTO);

            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
