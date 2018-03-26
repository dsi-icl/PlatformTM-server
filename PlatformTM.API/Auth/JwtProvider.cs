using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace PlatformTM.API.Auth
{
    public class JwtProvider
    {
        private readonly TokenAuthOptions _options;
        public JwtProvider(IOptions<TokenAuthOptions> options)
        {
            _options = options.Value;
        }


        public string GenerateJwt(ClaimsIdentity claimsIdentity){
            var now = DateTime.UtcNow;

            var _issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.HmacSecretKey));

            //Generate Token
            var jwtHandler = new JwtSecurityTokenHandler();
            var jws = jwtHandler.CreateEncodedJwt(new SecurityTokenDescriptor
            {
                Issuer = _options.Issuer,
                IssuedAt = now,
                NotBefore = now,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(_issuerSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = claimsIdentity,
                Expires = now.Add(_options.ExpiresSpan)
            });
            return jws;
        }
    }
}
