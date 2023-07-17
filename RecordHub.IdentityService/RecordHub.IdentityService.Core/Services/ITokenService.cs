using RecordHub.IdentityService.Domain.Data.Entities;
using System.Security.Claims;

namespace RecordHub.IdentityService.Core.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user, IEnumerable<Claim> additionalClaims = null);
    }
}
