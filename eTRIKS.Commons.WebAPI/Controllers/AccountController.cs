using eTRIKS.Commons.Core.Application.AccountManagement;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;


namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("api/accounts")]
    public class AccountController : Controller
    {
        private readonly UserAccountService _userAccountService;
        //private readonly UserManager<UserAccount> _userManager;
        //private readonly SignInManager<UserAccount> _signInManager;
        //private readonly UserManager<ApplicationUser, Guid> _userManager;

        public AccountController(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userAccountService.RegisterUser(userDTO);
            if (result.Succeeded)
            {
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                //await _signInManager.SignInAsync(user, isPersistent: false);
                //_logger.LogInformation(3, "User created a new account with password.");
                //return RedirectToAction(nameof(HomeController.Index), "Home");

                var psk = await _userAccountService.GetUserPsk(userDTO);
                return Ok(new { PSK = psk });
            }
            return GetErrorResult(result);
            
        }
        private IActionResult GetErrorResult(IdentityResult result)
        {
            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
