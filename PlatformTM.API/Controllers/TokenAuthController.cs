using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlatformTM.API.Auth;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services.UserManagement;

namespace PlatformTM.API.Controllers
{
    [Route("api/[controller]")]
    public class TokenAuthController : Controller
    {
        private readonly TokenAuthOptions TokenAuthOptions;
        private readonly UserAccountService _accountService;
       
        
        public TokenAuthController(TokenAuthOptions tokenOptions, UserAccountService userService)
        {
            this.TokenAuthOptions = tokenOptions;
            _accountService = userService;

        }


        [HttpPost]
        public async Task<dynamic> GetAuthToken([FromBody] UserDTO userDTO)
        {
            var result = await _accountService.SignIn(userDTO);
            // Obviously, at this point you need to validate the username and password against whatever system you wish.
            if (result.Succeeded)
            {
                DateTime expires = DateTime.UtcNow.AddMinutes(2);
                var token = GenerateToken(userDTO, expires);
                return new { authenticated = true, entityId = 1, token = token, tokenExpires = expires };
            }
            return new { authenticated = false };
        }

        private string GenerateToken(UserDTO userDTO, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(userDTO.Username, "TokenAuth"),
                new[] {
                    new System.Security.Claims.Claim("ID", userDTO.Username)
                }
            );

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = TokenAuthOptions.Issuer,
                Audience = TokenAuthOptions.Audience,
                SigningCredentials = TokenAuthOptions.SigningCredentials,
                Subject = identity,
                Expires = expires
            });
            return handler.WriteToken(securityToken);
        }
    }
}