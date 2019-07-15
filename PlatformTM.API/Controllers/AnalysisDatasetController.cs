using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Core.Domain.Model.Users.Datasets;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.DTOs.Export;
using PlatformTM.Services.Services;
using PlatformTM.Services.Services.UserManagement;

namespace PlatformTM.API.Controllers
{
    [Route("analysisdatasets")]
    public class AnalysisDatasetController : Controller
    {
		private readonly AnalysisDatasetService _datasetService;
        private readonly UserAccountService _accountService;

        public AnalysisDatasetController(AnalysisDatasetService datasetService, UserAccountService accountService)
        {
			_datasetService = datasetService;
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult GetUserDatasets()
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
            if (!User.Identity.IsAuthenticated)
                return null;
			var datasets =  _datasetService.GetUserDatasets(userId);
            return new OkObjectResult(datasets);
        }

        [HttpGet("{datasetId}", Name = "GetUserDatasetById")]
		public  AnalysisDatasetDTO GetUserDataset(string datasetId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
			return !User.Identity.IsAuthenticated ? null : _datasetService.GetUserDataset(datasetId, userId);
        }

		[HttpDelete]
        [Route("{datasetId}")]
        public void DeleteDataset(string datasetId)
        {
			//Get userId check that current user CAN delete this dataset either by being its owner
			_datasetService.DeleteDataset(datasetId);
        }

        [HttpGet]
		[Route("{datasetId}/files/{fileId}/export")]
		public IActionResult ExportFile(string datasetId, string fileId){
			
			return Ok();
		}

   //     [HttpPost]
   //     public IActionResult AddUserDataset([FromBody] UserDatasetDTO dto)
   //     {
   //         UserDatasetDTO addedUserDataset = null;
   //         if (!User.Identity.IsAuthenticated)
   //             return null;
   //         var userId = User.FindFirst(ClaimTypes.UserData).Value;
			//addedUserDataset = _datasetService.AddUserDataset(dto,userId);

        //    if (addedUserDataset != null)
        //        return new CreatedAtActionResult("GET", "GetUserDatasetById", new { datasetId = addedUserDataset.Id }, addedUserDataset);

        //    return new StatusCodeResult(StatusCodes.Status409Conflict);
        //}

        [HttpPut("{datasetId}")]
        public IActionResult UpdateUserDataset(Guid datasetId, [FromBody] AnalysisDatasetDTO dataset)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return null;

                var userId = User.FindFirst(ClaimTypes.UserData).Value;
				_datasetService.UpdateDataset(dataset,userId);
                return new AcceptedResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }
    }
}
