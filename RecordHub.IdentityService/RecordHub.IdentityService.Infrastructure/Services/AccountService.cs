using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.Exceptions;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ITokenService tokenService;
        public AccountService(
          UserManager<User> userManager,
          SignInManager<User> signInManager,
          ITokenService tokenService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokenService = tokenService;
        }
        public async Task<string> LoginAsync(LoginModel model, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByNameAsync(model.Email) ??
                throw new UserNotFoundException();
            var credetialsResult = await userManager.CheckPasswordAsync(
                user, model.Password);

            if (!credetialsResult)
            {
                throw new InvalidCredentialsException();
            }

            return tokenService.GenerateJwtToken(user);
        }

        public async Task RegisterAsync(RegisterModel model, HttpContext httpContext, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname,
                PhoneNumber = model.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new InvalidPasswordRequirementsExceptions(result.Errors);
            }

            result = await userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!result.Succeeded)
            {
                throw new AddingToRoleException(result.Errors);
            }
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
