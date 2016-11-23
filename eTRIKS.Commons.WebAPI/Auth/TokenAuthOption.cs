using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.WebAPI.Auth
{
    public class TokenAuthOption
    ////{
    ////    public string Audience { get; set; }
    ////    public string Issuer { get; set; }
    ////    public SigningCredentials SigningCredentials { get; set; }
    ////    public static RsaSecurityKey Key { get; } = new RsaSecurityKey(RSAKeyUtils.GetRandomKey());
    ////    public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
    ////    public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromMinutes(20);
    
    ////}

        {
    public static string Audience { get; } = "ExampleAudience";
    public static string Issuer { get; } = "ExampleIssuer";
    public static RsaSecurityKey Key { get; } = new RsaSecurityKey(RSAKeyHelper.GenerateKey());
    public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
    public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromMinutes(20);
}
}
