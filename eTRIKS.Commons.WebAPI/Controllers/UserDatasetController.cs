using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using eTRIKS.Commons.WebAPI.Extensions;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("api/mydatasets")]
    public class UserDatasetController : Controller
    {
        private readonly UserDatasetService _userDataService;
        private readonly UserAccountService _accountService;

        public UserDatasetController(UserDatasetService userDataService, UserAccountService accountService)
        {
            _userDataService = userDataService;
            _accountService = accountService;
        }

        [HttpGet("projects/{projectId}")]
        public List<UserDatasetDTO> GetUserDatasets(int projectId)
        {
            var userId = User.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            return _userDataService.GetUserDatasets(projectId, userId);

        }

        [HttpGet("{datasetId}", Name = "GetUserDatasetById")]
        public  UserDatasetDTO GetUserDataset(string datasetId)
        {
            var userId = User.GetUserId();
            return !User.Identity.IsAuthenticated ? null : _userDataService.GetUserDataset(datasetId, userId);
        }

        [HttpPost]
        public IActionResult AddUserDataset([FromBody] UserDatasetDTO dto)
        {
            UserDatasetDTO addedUserDataset = null;

            var userId = User.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            addedUserDataset = _userDataService.AddUserDataset(dto,userId);

            if (addedUserDataset != null)
            {
                //var response = Request.CreateResponse<UserDatasetDTO>(HttpStatusCode.Created, addedUserDataset);
                //string uri = Url.Link("GetUserDatasetById", new { datasetId = addedUserDataset.Id });
                //response.Headers.Location = new Uri(uri);
                //return response;
                return new CreatedAtActionResult("GET", "GetUserDatasetById", new { datasetId = addedUserDataset.Id }, addedUserDataset);
            }
            else
            {
                //var response = Request.CreateResponse(HttpStatusCode.Conflict);
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }

        [HttpPut("{datasetId}")]
        public IActionResult UpdateUserDataset(Guid datasetId, [FromBody] UserDatasetDTO dto)
        {
            try
            {
                var userId = User.GetUserId();
                if (!User.Identity.IsAuthenticated)
                    return null;
                _userDataService.UpdateUserDataset(dto,userId);
                return new StatusCodeResult(StatusCodes.Status202Accepted);
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }
        }
    }
}
