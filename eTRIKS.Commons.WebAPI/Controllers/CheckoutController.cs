using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
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
            var datasets = _checkoutService.CreateCheckoutDatasets(cartId,userId);
            if (datasets != null)
                return Ok(datasets);
            return NotFound();
        }
    }
}
