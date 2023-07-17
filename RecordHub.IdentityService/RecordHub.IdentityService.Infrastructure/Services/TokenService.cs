using RecordHub.IdentityService.Core.Services;
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
        public string GenerateJwtToken(User user, IEnumerable<Claim> additionalClaims = null)
        {
            List<Claim> claims = new List<Claim>() {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email, user.Email),
                new Claim (JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim (JwtRegisteredClaimNames.Name, user.Name),
                new Claim (ClaimTypes.Surname, user.Surname),
                new Claim (ClaimTypes.MobilePhone, user.PhoneNumber),
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims);
            }

            JwtSecurityToken token = new TokenBuilder()
              .AddAudience(_config.Audience)
              .AddIssuer(_config.Issuer)
              .AddExpiry(_config.TokenLifeTime)
              .AddKey(_config.Key)
              .AddClaims(claims)
              .Build();

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }
    }
}
