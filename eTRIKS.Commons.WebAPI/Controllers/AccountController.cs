using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Service.Services.UserServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [RoutePrefix("api/accounts")]
    public class AccountController : ApiController
    {
        private UserAccountService userAccountService;

        public AccountController(UserAccountService userService)
        {
            userAccountService = userService;
        }

        [Route("signup")]
        public async Task<IHttpActionResult> CreateUser(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            //IdentityResult addUserResult = await this.AppUserManager.CreateAsync(user, createUserModel.Password);

            bool succeeded = await userAccountService.RegisterUser(userDTO);

            if (!succeeded)
            {
                return BadRequest();//GetErrorResult(addUserResult);
            }

            //Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            //return Created(locationHeader, TheModelFactory.Create(user));
            //ApplicationUser user = await userAccountService.FindUser(userModel.UserName, userModel.Password);
            string psk = await userAccountService.GetUserPSK(userDTO);
            return Ok(new { PSK = psk });
        }

        //private IHttpActionResult GetErrorResult(IdentityResult result)
        //{
        //    if (result == null)
        //    {
        //        return InternalServerError();
        //    }

        //    if (!result.Succeeded)
        //    {
        //        if (result.Errors != null)
        //        {
        //            foreach (string error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error);
        //            }
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            // No ModelState errors are available to send, so just return an empty BadRequest.
        //            return BadRequest();
        //        }

        //        return BadRequest(ModelState);
        //    }

        //    return null;
        //}
    }
}
