using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.DataAccess;
using eTRIKS.Commons.DataAccess.UserManagement;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.Services.UserServices
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        private readonly IUserRepository<ApplicationUser> _userRepository;
        public SimpleAuthorizationServerProvider(IUserRepository<ApplicationUser> userRepository)
        {
            _userRepository = userRepository;
        }

        //public SimpleAuthorizationServerProvider(IUserDbContext identityDbContext)
        //{

        //}

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            ApplicationUser appUser = null;

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // // using (UserAuthRepository _repo = new UserAuthRepository())
            ////  {

            // //pass uoW and get from it the repository for authentication uoW takes care of instantiating the repo
            // // UserAuthRepository _repo = new UserAuthRepository();
            appUser = await _userRepository.FindUser(context.UserName, context.Password);

            if (appUser == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            //  }

            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            identity.AddClaim(new Claim("PSK", appUser.PSK));

            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "userName", context.UserName
                    }
               });

            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {

            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
    }
}