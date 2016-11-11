using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountController : Controller
    {
        private readonly UserAccountService _userAccountService;
        //private readonly UserManager<ApplicationUser, Guid> _userManager;

        public AccountController(UserAccountService userService)
        {
            _userAccountService = userService;
            //_userManager = userManager;

        }

        [Route("signup")]
        public async Task<IHttpActionResult> CreateUser(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

           
            //IdentityResult addUserResult = _userManager.CreateAsync(user, createUserModel.Password);

            IdentityResult addUserResult = await _userAccountService.RegisterUser(userDTO);

            if (!addUserResult.Succeeded)
            {
                return GetErrorResult(addUserResult);
            }

            //Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            //return Created(locationHeader, TheModelFactory.Create(user));
            //ApplicationUser user = await userAccountService.FindUser(userModel.UserName, userModel.Password);
            string psk = await _userAccountService.GetUserPsk(userDTO);
            return Ok(new { PSK = psk });
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
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
