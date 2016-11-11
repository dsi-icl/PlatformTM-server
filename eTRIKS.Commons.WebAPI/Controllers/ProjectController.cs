using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
            var userId = User.Identity.GetUserId();
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
        public async Task<HttpResponseMessage> AddProject([FromBody] ProjectDTO projectDTO)
        {
            ProjectDTO addedProject = null;

            var userId = User.Identity.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            //var account = await _accountService.(name);
            addedProject = await _projectService.AddProject(projectDTO,userId);


            if (addedProject != null)
            {
                var response = Request.CreateResponse<ProjectDTO>(HttpStatusCode.Created, addedProject);
                string uri = Url.Link("GetProjectById", new { projectId = addedProject.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            else
            {
                var response = Request.CreateResponse(HttpStatusCode.Conflict);
                return response;
            }
        }

        [HttpPut]
        [Route("api/projects/{projectId}")]
        public HttpResponseMessage UpdateProject(int projectId, [FromBody] ProjectDTO projectDTO)
        {
            try
            {
                _projectService.UpdateProject(projectDTO, projectId);
                var response = Request.CreateResponse<ProjectDTO>(HttpStatusCode.Accepted, projectDTO);
                string uri = Url.Link("GetProjectById", new { projectId = projectDTO.Id });
                response.Headers.Location = new Uri(uri);
                return response;
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict);
            }
        }
    }
}
