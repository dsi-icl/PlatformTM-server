using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PlatformTM.API.Auth;
using PlatformTM.API.Services;
using PlatformTM.Core.Application.AccountManagement;
using PlatformTM.Models.Services.UserManagement;


namespace PlatformTM.API.Controllers
{
    [Route("token")]
    public class TokenController : Controller
    {
        private readonly TokenAuthOptions _options;
        private readonly UserAccountService _accountService;
        private readonly JwtProvider _tokenProvider;

        public TokenController(IOptions<TokenAuthOptions> options, UserAccountService userService, JwtProvider tokenProvider){
            _options = options.Value;
            _accountService = userService;
            _tokenProvider = tokenProvider;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenAsync([FromForm] CredentialsViewModel credentialsVM)
        {
            var user = Authenticate(credentialsVM).Result;
            if (user == null)
                return new UnauthorizedResult();

            var signInResult = await _accountService.Login(credentialsVM.Username, credentialsVM.Password);
            if(!signInResult.Succeeded){
                return StatusCode(403,"Account not confirmed");
            }

            

            var claimsPrincipal = await _accountService.GetClaimsPrincipleForUser(user);
            var userData = _accountService.GetUserInfo(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)).Result;
            var jws = _tokenProvider.GenerateJwt((ClaimsIdentity)claimsPrincipal.Identity);

            var response = new
            {
                access_token = jws,
                token_result = "success",
                user = userData,
                expires_in = (int)_options.ExpiresSpan.TotalMinutes
            };

            var result = JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return new OkObjectResult(result);
        }

        private async Task<UserAccount> Authenticate(CredentialsViewModel loginVM){
            return  await _accountService.FindUserAsync(loginVM.Username, loginVM.Password);
        }

        private string _getSuccessResult(){
            return null;
        }

    }
}
