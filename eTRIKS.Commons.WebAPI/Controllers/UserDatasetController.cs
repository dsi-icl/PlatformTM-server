using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Service.Services.UserManagement;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/mydatasets")]
    public class UserDatasetController : Controller
    {
        private readonly UserDatasetService _userDataService;
        private readonly UserAccountService _accountService;

        public UserDatasetController(UserDatasetService userDataService, UserAccountService accountService)
        {
            _userDataService = userDataService;
            _accountService = accountService;
        }

        [HttpGet]
        [Route("projects/{projectAcc}")]
        public async Task<List<UserDatasetDTO>> GetUserDatasets(string projectAcc)
        {
            var userId = User.Identity.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            //var account = await _accountService.FindByNameAsync(name);
            return _userDataService.GetUserDatasets(projectAcc,userId);

        }

        [HttpGet]
        [Route("{datasetId}", Name = "GetUserDatasetById")]
        public  UserDatasetDTO GetUserDataset(string datasetId)
        {
            var userId = User.Identity.GetUserId();
            return !User.Identity.IsAuthenticated ? null : _userDataService.GetUserDataset(datasetId, userId);
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage AddUserDataset([FromBody] UserDatasetDTO dto)
        {
            UserDatasetDTO addedUserDataset = null;

            var userId = User.Identity.GetUserId();
            if (!User.Identity.IsAuthenticated)
                return null;
            addedUserDataset = _userDataService.AddUserDataset(dto,userId);

            if (addedUserDataset != null)
            {
                var response = Request.CreateResponse<UserDatasetDTO>(HttpStatusCode.Created, addedUserDataset);
                string uri = Url.Link("GetUserDatasetById", new { datasetId = addedUserDataset.Id });
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
        [Route("{datasetId}")]
        public HttpResponseMessage UpdateUserDataset(Guid datasetId, [FromBody] UserDatasetDTO dto)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                if (!User.Identity.IsAuthenticated)
                    return null;
                _userDataService.UpdateUserDataset(dto,userId);
                var response = Request.CreateResponse<UserDatasetDTO>(HttpStatusCode.Accepted, dto);
                string uri = Url.Link("GetUserDatasetById", new { datasetId = dto.Id });
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
