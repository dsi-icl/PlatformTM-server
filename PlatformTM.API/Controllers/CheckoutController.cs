using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Services.DTOs.Export;
using PlatformTM.Services.Services;

namespace PlatformTM.API.Controllers
{
    [Route("checkout")]
    public class CheckoutController : Controller
    {
        private readonly CheckoutService _checkoutService;

		public CheckoutController(CheckoutService checkoutService)
        {
			_checkoutService = checkoutService;
        }

        [HttpGet("{cartId}")]
        public IActionResult CheckoutDatasets(string cartId)
        {
            var userId = User.FindFirst(ClaimTypes.UserData).Value;
			var exportFiles = _checkoutService.GetCheckoutResults(cartId, userId);
            //var datasets = _checkoutService.CreateCheckoutDatasets(cartId,userId);
			if (exportFiles != null)
				return Ok(exportFiles);
            return NotFound();
        }

		[HttpPost]
		public IActionResult SaveToDataset([FromBody] AnalysisDatasetDTO aDatasetDTO)
        {
			AnalysisDatasetDTO newDataset = null;


            if (!User.Identity.IsAuthenticated)
                return Unauthorized();
			//if(ModelState.IsValid)

            var userId = User.FindFirst(ClaimTypes.UserData).Value;
			newDataset = _checkoutService.SaveToDataset(aDatasetDTO, userId);

			//if (newDataset != null)
			//    return new CreatedAtRouteResult("GetProjectByAcc", new { projectId = addedProject.Id }, addedProject);

			//return new StatusCodeResult(StatusCodes.Status409Conflict);
			return Ok();

        }
    }
}
