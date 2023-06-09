using Microsoft.AspNetCore.Http;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Models;

namespace RecordHub.IdentityService.Core.Services
{
    public interface IAccountService
    {
        Task<AuthTokenDTO> LoginAsync(
            LoginModel model,
            HttpContext httpContext,
            CancellationToken cancellationToken = default);

        Task RegisterAsync(
            RegisterModel model,
            HttpContext httpContext,
            CancellationToken cancellationToken = default);

        Task SentEmailVerificationAsync(
            HttpContext httpContext,
            CancellationToken token = default);

        Task<bool> VerifyEmailAsync(
          string token,
          HttpContext httpContext,
          CancellationToken cancellationToken = default);

        Task SignOut(
            HttpContext httpContext,
            CancellationToken token = default);
    }
}
