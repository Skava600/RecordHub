using Microsoft.AspNetCore.Http;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Models;
using RecordHub.IdentityService.Core.Services;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        public Task<AuthTokenDTO> LoginAsync(LoginModel model, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RegisterAsync(RegisterModel model, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task SentEmailVerificationAsync(HttpContext httpContext, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task SignOut(HttpContext httpContext, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmailAsync(string token, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
