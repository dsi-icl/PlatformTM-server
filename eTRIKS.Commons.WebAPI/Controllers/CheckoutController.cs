using System.Collections.Generic;
using eTRIKS.Commons.Service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace eTRIKS.Commons.WebAPI.Controllers
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
