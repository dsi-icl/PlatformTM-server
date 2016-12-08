using eTRIKS.Commons.Service.DTOs;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services.UserManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace eTRIKS.Commons.WebAPI.Auth
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenAuthOptions _options;
        private readonly UserAccountService _accountService;

        public TokenProviderMiddleware(RequestDelegate next, UserAccountService userService, IOptions<TokenAuthOptions> options)
        {
            _next = next;
            _options = options.Value;
            _accountService = userService;
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Endpoint, StringComparison.Ordinal))
            {
                return _next(context);
            }

            //if (context.Request.Form["granttype"] != "passwrod")
            //{
            //    return BadRequestResult(_next);
            //}

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
        }

        private async Task GenerateToken(HttpContext context)
        {

            var appUser = await _accountService.FindUserAsync(context.Request.Form["userName"], context.Request.Form["password"]);

            if (appUser == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;   
            }
            var identity = await _accountService.GetClaimsPrincipleForUser(appUser);
            //var result = await _accountService.SignIn(userDTO);
            // Obviously, at this point you need to validate the username and password against whatever system you wish.
            //if (result.Succeeded)
            //{
            //    DateTime expires = DateTime.UtcNow.AddMinutes(2);
            //    var token = GenerateToken(userDTO, expires);
            //    return new { authenticated = true, entityId = 1, token = token, tokenExpires = expires };
            //}
            //return new { authenticated = false };
            //Create ClaimsIdentity
            //ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            //identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            //identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Account.UserId.ToString()));
            ////identity.AddClaim(new Claim("PSK", appUser.Account.PSK));

            //ClaimsIdentity identity = new ClaimsIdentity(
            //    new GenericIdentity(userDTO.Username, "TokenAuth"),
            //    new[] {
            //        new System.Security.Claims.Claim("ID", userDTO.Username)
            //    }
            //);
           
            var now = DateTime.UtcNow;
            //Generate Token
            var jwtHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtHandler.CreateToken(new SecurityTokenDescriptor
            {
                
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = _options.SigningCredentials,
                Subject = (ClaimsIdentity)identity.Identity,
                NotBefore = now,
                Expires = now.Add(_options.ExpiresSpan)
            });
            var encodedToken = jwtHandler.WriteToken(securityToken);

            var response = new
            {
                access_token = encodedToken,
                expires_in = (int)_options.ExpiresSpan.TotalSeconds
            };
            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
    }
}
