using System;
using System.Collections.Generic;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using eTRIKS.Commons.WebAPI.Extensions;
using System.Collections;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private ProjectService _projectService;
        private readonly UserAccountService _accountService;


        public ProjectController(ProjectService projectService, UserAccountService accountService)
        {
            _projectService = projectService;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("api/projects")]
        public IEnumerable<ProjectDTO> Get()
        {
            var userId = User.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            return _projectService.GetProjects(userId);
        }

        [HttpGet]
        [Route("api/projects/accession/{projectId}", Name = "GetProjectByAcc")]
        public ProjectDTO GetProjectFull(int projectId)
        {
            return _projectService.GetProjectFullDetails(projectId);
        }
        [HttpGet]
        [Route("api/projects/id/{projectId}", Name = "GetProjectById")]
        public ProjectDTO GetProject(int projectId)
        {
            return _projectService.GetProjectById(projectId);
        }

        [HttpGet]
        [Route("api/projects/{projectId}/activities")]
        public IEnumerable<ActivityDTO> GetProjectActivities(int projectId)
        {
            return _projectService.GetProjectActivities(projectId);
        }

        [HttpPost]
        [Route("api/projects")]
        public IActionResult AddProject([FromBody] ProjectDTO projectDTO)
        {
            ProjectDTO addedProject = null;

            var userId = User.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            //var account = await _accountService.(name);
            addedProject = _projectService.AddProject(projectDTO,userId);


            if (addedProject != null)
            {
                //var response = Request.CreateResponse<ProjectDTO>(HttpStatusCode.Created, addedProject);
                //string uri = Url.Link("GetProjectById", new { projectId = addedProject.Id });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetProjectById", new { projectId = addedProject.Id }, addedProject);
            }
            else
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpPut]
        [Route("api/projects/{projectId}")]
        public IActionResult UpdateProject(int projectId, [FromBody] ProjectDTO projectDTO)
        {
            try
            {
                _projectService.UpdateProject(projectDTO, projectId);
                //var response = Request.CreateResponse<ProjectDTO>(HttpStatusCode.Accepted, projectDTO);
                //string uri = Url.Link("GetProjectById", new { projectId = projectDTO.Id });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetProjectById", new { projectId = projectDTO.Id }, projectDTO);

            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpGet]
        [Route("api/projects/{projectId}/assays")]
        public List<AssayDTO> GetAssays(int projectId)
        {
            return _projectService.GetProjectAssays(projectId);
        }
    }
}
