using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services.HelperService;
using PlatformTM.Models.Services.UserManagement;

namespace PlatformTM.API.Controllers
{
    [Route("accounts")]
    public class AccountController : Controller
    {
        private readonly UserAccountService _userAccountService;
        readonly EmailService _emailService; 

        public AccountController(UserAccountService userAccountService, EmailService emailService)
        {
            _userAccountService = userAccountService;
            _emailService = emailService;
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
                var user = await _userAccountService.FindUserAsync(userDTO.Username, userDTO.Password);
                // Send an email with this link
                var code = await _userAccountService.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Action(
                    action: nameof(AccountController.ConfirmEmail),
                    controller: "Account",
                    values: new { userId=user.Id.ToString(), code },
                    protocol: Request.Scheme);
                var emailRes = await _emailService.SendEmailAsync(user.User.Email, "Confirm your PlatformTM account","Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                await _userAccountService.SignInAsync(user, isPersistent: false);
               
                return Ok();
            }
            return GetErrorResult(result);
            
        }

		[HttpGet("logout")]
		[AllowAnonymous]
		public IActionResult Logout(){
			return new SignOutResult();
		}

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return new RedirectResult("/authentication/failed.html");
            }
            var user = await _userAccountService.FindByIdAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult("User id " + userId + "not found");
            }
            var result = await _userAccountService.ConfirmEmailAsync(user, code);
            return new RedirectResult("/app/#/verified");
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userAccountService.FindByEmailAsync(model.Email);
        //        if (user == null || !(await _userAccountService.IsEmailConfirmedAsync(user)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        //        }

        //        // For more information on how to enable account confirmation and password reset please
        //        // visit https://go.microsoft.com/fwlink/?LinkID=532713
        //        var code = await _userAccountService.GeneratePasswordResetTokenAsync(user);
        //        var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
        //        await _emailService.SendEmailAsync(model.Email, "Reset Password",
        //           $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
        //        //Please check your email to reset your password.
        //        //return RedirectToAction(nameof(ForgotPasswordConfirmation));
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return new BadRequestResult();
        //}

        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult ResetPassword(string code = null)
        //{
        //    if (code == null)
        //    {
        //        throw new ApplicationException("A code must be supplied for password reset.");
        //    }
        //    var model = new ResetPasswordViewModel { Code = code };
        //    return View(model);
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction(nameof(ResetPasswordConfirmation));
        //    }
        //    var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(ResetPasswordConfirmation));
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //    }

        //    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
        //    var email = user.Email;
        //    await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

        //    StatusMessage = "Verification email sent. Please check your email.";
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet("currentuser")]
        public ActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User.Identity.IsAuthenticated)
            {
                return new OkObjectResult(_userAccountService.GetUserInfo(userId));
            }
            return new UnauthorizedResult();
        }

        [HttpGet("userclaims")]
        public ActionResult GetCurrentUserClaims()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User.Identity.IsAuthenticated)
            {
                return new OkObjectResult(_userAccountService.GetClaimsForUser(userId));
            }
            return new UnauthorizedResult();
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
