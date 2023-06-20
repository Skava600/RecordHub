using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Core.Services
{
    public interface IAccountService
    {
        Task<string> LoginAsync(
            LoginModel model,
            CancellationToken cancellationToken = default);

        Task RegisterAsync(
            RegisterModel model,
            CancellationToken cancellationToken = default);

        Task SendEmailVerificationAsync(string? userId,
            CancellationToken token = default);

        Task<UserDTO> GetUserInfoAsync(string? userId,
            CancellationToken token = default);

        Task VerifyEmailAsync(
          string token,
          string? userId,
          CancellationToken cancellationToken = default);
    }
}
