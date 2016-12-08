using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.WebAPI.Auth
{
    public class TokenAuthOptions
    {
        public string Endpoint { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; } 
        public SigningCredentials SigningCredentials { get; set; } //= new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        public TimeSpan ExpiresSpan { get; } = TimeSpan.FromMinutes(20);
    }
}
