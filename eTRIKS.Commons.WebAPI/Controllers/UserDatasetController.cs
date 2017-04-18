using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using eTRIKS.Commons.WebAPI.Extensions;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("datasets")]
    public class UserDatasetController : Controller
    {
        private readonly UserDatasetService _userDataService;
        private readonly UserAccountService _accountService;

        public UserDatasetController(UserDatasetService userDataService, UserAccountService accountService)
        {
            _userDataService = userDataService;
            _accountService = accountService;
        }

        [HttpGet("user")]
        public IActionResult GetUserDatasets()
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            if (!User.Identity.IsAuthenticated)
                return null;
            var datasets =  _userDataService.GetUserDatasets(userId);
            return new OkObjectResult(datasets);
        }

        [HttpGet("{datasetId}", Name = "GetUserDatasetById")]
        public  UserDatasetDTO GetUserDataset(string datasetId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            return !User.Identity.IsAuthenticated ? null : _userDataService.GetUserDataset(datasetId, userId);
        }

        [HttpPost]
        public IActionResult AddUserDataset([FromBody] UserDatasetDTO dto)
        {
            UserDatasetDTO addedUserDataset = null;
            if (!User.Identity.IsAuthenticated)
                return null;
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            addedUserDataset = _userDataService.AddUserDataset(dto,userId);

            if (addedUserDataset != null)
                return new CreatedAtActionResult("GET", "GetUserDatasetById", new { datasetId = addedUserDataset.Id }, addedUserDataset);

            return new StatusCodeResult(StatusCodes.Status409Conflict);
        }

        [HttpPut("{datasetId}")]
        public IActionResult UpdateUserDataset(Guid datasetId, [FromBody] UserDataset dataset)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return null;

                var userId = User.FindFirst(ClaimTypes.UserData).Value;
                _userDataService.UpdateUserDataset(dataset,userId);
                return new AcceptedResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
