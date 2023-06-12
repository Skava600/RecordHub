using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Core.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
    }
}
