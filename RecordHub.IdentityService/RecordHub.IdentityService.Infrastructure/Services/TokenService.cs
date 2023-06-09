using Microsoft.AspNetCore.Http;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        public Task<AuthTokenDTO> GenerateDTO(User user, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
