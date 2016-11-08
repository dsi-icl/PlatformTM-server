using eTRIKS.Commons.Service.Services.UserManagement;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace eTRIKS.Commons.Service.Services.Authentication
{
    public class CustomOAuthServerProvider : OAuthAuthorizationServerProvider
    {
        //private readonly IUserRepository<ApplicationUser,IdentityResult> _userRepository;
        private readonly UserAccountService _accountService;


        public CustomOAuthServerProvider(UserAccountService usraccservice)
        {
            _accountService = usraccservice;
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
            return Task.FromResult<object>(null);
        }
        
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var allowedOrigin = "*";
            UserAccount appUser = null;

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // // using (UserAuthRepository _repo = new UserAuthRepository())
            ////  {

            // //pass uoW and get from it the repository for authentication uoW takes care of instantiating the repo
            // // UserAuthRepository _repo = new UserAuthRepository();
            appUser = await _accountService.FindUserAsync(context.UserName, context.Password);

            if (appUser == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            //  }

            ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "User"));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, appUser.Account.UserId.ToString()));
            identity.AddClaim(new Claim("PSK", appUser.Account.PSK));

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