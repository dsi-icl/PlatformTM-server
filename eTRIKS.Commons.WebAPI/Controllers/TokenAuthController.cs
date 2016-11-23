using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using eTRIKS.Commons.WebAPI.Auth;



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using eTRIKS.Commons.Service.Services.UserManagement;
//using System.Threading.Tasks;

namespace eTRIKS.Commons.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class TokenAuthController : Controller
    {
        private readonly TokenAuthOption TokenAuthOptions;
        private readonly UserAccountService _accountService;
        //UserAccountService _accountService { get; set; }
        
        public TokenAuthController(TokenAuthOption tokenOptions, UserAccountService userService)
        {
            this.TokenAuthOptions = TokenAuthOptions;
            _accountService = userService;
            //this.bearerOptions = options.Value;
            //this.signingCredentials = signingCredentials;
        }
      

        //***********************************************************************************   Authentication   (GetAuthToken)
        [HttpPost]
        public string GetAuthToken(User user)
        {
            var existUser = UserStorage.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            if (existUser != null)
            {
                var requestAt = DateTime.Now;
                var expiresIn = requestAt + TokenAuthOption.ExpiresSpan;
                var token = GenerateToken(existUser, expiresIn);

                return JsonConvert.SerializeObject(new
                {
                    stateCode = 1,
                    requertAt = requestAt,
                    expiresIn = TokenAuthOption.ExpiresSpan.TotalSeconds,
                    accessToken = token
                });
            }
            else
            {
                return JsonConvert.SerializeObject(new { stateCode = -1, errors = "Username or password is invalid" });
            }
        }

                
        ////////[HttpPost]
        ////////public async Task<dynamic> Post([FromBody] User user)
        ////////// public dynamic Post([FromBody] AuthRequest req)
        ////////{
        ////////    UserAccount appUser = null;
        ////////    var allowedOrigin = "*";
        ////////    appUser = await _accountService.FindUserAsync(user.username, user.password);

        ////////    // appUser = _accountService(u => u.Username == user.Username && u.Password == user.Password);
        ////////    // Obviously, at this point you need to validate the username and password against whatever system you wish.
        ////////    //if ((req.username == "TEST" && req.password == "TEST") || (req.username == "TEST2" && req.password == "TEST"))

        ////////    /////following is ddedd (start)
        ////////    //var allowedOrigin = "*";
        ////////    //    UserAccount appUser = null;

        ////////    //appUser = await _userAccountService.FindUserAsync(req.username, req.password);

        ////////    if (appUser != null)

        ////////    /////following is ddedd (finish)
        ////////    {
        ////////        DateTime? expires = DateTime.UtcNow.AddMinutes(2);
        ////////        var token = GenerateToken(user.username, expires);
        ////////        // var token = GetToken(req.username, expires);
        ////////        return new { authenticated = true, entityId = 1, token = token, tokenExpires = expires };
        ////////    }

        ////////    {
        ////////        user.SetError("invalid_grant", "The user name or password is incorrect.");
        ////////        return;
        ////////    }
        ////////    return new { authenticated = false };
        ////////}



        //***********************************************************************************     GenerateToken (GetToken)
        private string GenerateToken(User user, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(user.Username, "TokenAuth"),
                new[] {
                    new Claim("ID", user.ID.ToString())
                }
            );

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = TokenAuthOption.Issuer,
                Audience = TokenAuthOption.Audience,
                SigningCredentials = TokenAuthOption.SigningCredentials,
                Subject = identity,
                Expires = expires
            });
            return handler.WriteToken(securityToken);
        }
    }
    //***********************************************************************************     USER
    public class User
    {
        public Guid ID { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
    //***********************************************************************************     UserStorage  (like userService)
    public static class UserStorage
    {
        public static List<User> Users { get; set; } = new List<User> {
            new User {ID=Guid.NewGuid(),Username="user1",Password = "user1psd" },
            new User {ID=Guid.NewGuid(),Username="user2",Password = "user2psd" },
            new User {ID=Guid.NewGuid(),Username="user3",Password = "user3psd" }
        };
    }



    // *********************************************************************************************************************************************************************  [Authorize("Bearer")]
    //////[HttpGet]
    //////[Authorize("Bearer")]
    //////public dynamic Get()
    //////{
    //////    /* 
    //////    ******* WARNING WARNING WARNING ****** 
    //////    ******* WARNING WARNING WARNING ****** 
    //////    ******* WARNING WARNING WARNING ****** 
    //////    THIS METHOD SHOULD BE REMOVED IN PRODUCTION USE-CASES - IT ALLOWS A USER WITH 
    //////    A VALID TOKEN TO REMAIN LOGGED IN FOREVER, WITH NO WAY OF EVER EXPIRING THEIR
    //////    RIGHT TO USE THE APPLICATION.
    //////    Refresh Tokens (see https://auth0.com/docs/refresh-token) should be used to 
    //////    retrieve new tokens. 
    //////    ******* WARNING WARNING WARNING ****** 
    //////    ******* WARNING WARNING WARNING ****** 
    //////    ******* WARNING WARNING WARNING ****** 
    //////    */
    //////    bool authenticated = false;
    //////    string user = null;
    //////    int entityId = -1;
    //////    string token = null;
    //////    DateTime? tokenExpires = default(DateTime?);

    //////    var currentUser = HttpContext.User;
    //////    if (currentUser != null)
    //////    {
    //////        authenticated = currentUser.Identity.IsAuthenticated;
    //////        if (authenticated)
    //////        {
    //////            user = currentUser.Identity.Name;
    //////            foreach (Claim c in currentUser.Claims) if (c.Type == "EntityID") entityId = Convert.ToInt32(c.Value);
    //////            tokenExpires = DateTime.UtcNow.AddMinutes(2);
    //////            token = GenerateToken(currentUser.Identity.Name, tokenExpires);
    //////        }
    //////    }
    //////    return new { authenticated = authenticated, user = user, entityId = entityId, token = token, tokenExpires = tokenExpires };
    //////}


    //////public class AuthRequest
    //////{
    //////    public string username { get; set; }
    //////    public string password { get; set; }
    //////}


}