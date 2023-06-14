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

        Task SentEmailVerificationAsync(
            CancellationToken token = default);

        Task<bool> VerifyEmailAsync(
          string token,
          CancellationToken cancellationToken = default);
    }
}
