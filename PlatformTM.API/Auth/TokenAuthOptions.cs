using System;
using Microsoft.IdentityModel.Tokens;

namespace PlatformTM.API.Auth
{
    public class TokenAuthOptions
    {
        public string Endpoint { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; } 
        public SigningCredentials SigningCredentials { get; set; } //= new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        public TimeSpan ExpiresSpan { get; } = TimeSpan.FromMinutes(60);
    }
}
