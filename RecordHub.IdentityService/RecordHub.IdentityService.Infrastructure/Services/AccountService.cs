using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Exceptions;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Domain.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> roleManager;
        private readonly ITokenService tokenService;
        public AccountService(
          UserManager<User> userManager,
          RoleManager<IdentityRole<Guid>> roleManager,
          IMapper mapper,
          ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
            this.roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<UserDTO> GetUserInfoAsync(string? userId, CancellationToken token = default)
        {
            if (userId == null)
            {
                throw new UserNotFoundException();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            UserDTO userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }

        public async Task<string> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
        {
            var user = await userManager.FindByNameAsync(model.Email) ??
                throw new UserNotFoundException();

            var credetialsResult = await userManager.CheckPasswordAsync(
                user, model.Password);
            if (!credetialsResult)
            {
                throw new InvalidCredentialsException();
            }

            List<Claim> additionalClaims = new List<Claim>();

            string? rolename = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            if (rolename != null)
            {
                additionalClaims.Add(new Claim(ClaimTypes.Role, rolename));
            }

            return tokenService.GenerateJwtToken(user, additionalClaims);
        }

        public async Task RegisterAsync(RegisterModel model, CancellationToken cancellationToken = default)
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

            cancellationToken.ThrowIfCancellationRequested();

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

        public Task SendEmailVerificationAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
