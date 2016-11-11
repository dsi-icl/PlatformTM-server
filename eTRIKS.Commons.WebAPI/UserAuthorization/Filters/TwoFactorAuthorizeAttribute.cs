using eTRIKS.Commons.WebAPI.UserAuthorization.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace eTRIKS.Commons.WebAPI.UserAuthorization.Filters
{
    public class TwoFactorAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, System.Threading.CancellationToken cancellationToken)
        {
            var principal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            var preSharedKey = principal.FindFirst("PSK").Value;
            bool hasValidTotp = OtpHelper.HasValidTotp(actionContext.Request, preSharedKey);

            if (hasValidTotp)
            {
                return Task.FromResult<object>(null);
            }
            else
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new CustomError() { Code = 100, Message = "One Time Password is Invalid" });
                return Task.FromResult<object>(null);
            }
        }
    }

    public class CustomError
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
