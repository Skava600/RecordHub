using Microsoft.AspNetCore.Http;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Core.Services
{
    public interface ITokenService
    {
        Task<AuthTokenDTO> GenerateDTO(User user,
            HttpContext httpContext,
            CancellationToken cancellationToken = default);
    }
}
