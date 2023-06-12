using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Constants;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Infrastructure.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtConfig _config;
        public TokenService(JwtConfig jwtConfig)
        {
            this._config = jwtConfig;
        }
        public string GenerateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>() {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email, user.Email),
                new Claim (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            };
            JwtSecurityToken token = new TokenBuilder()
              .AddAudience(_config.Audience)
              .AddIssuer(_config.Issuer)
              .AddExpiry(Constants.TokenLifeTime)
              .AddKey(_config.Key)
              .AddClaims(claims)
              .Build();
            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);


            return accessToken;
        }
    }
}
