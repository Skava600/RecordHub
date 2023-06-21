using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Exceptions;
using RecordHub.IdentityService.Core.Publishers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.Shared.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IPublisher<MailData> _mailPublisher;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ITokenService tokenService;
        public AccountService(
          UserManager<User> userManager,
          RoleManager<IdentityRole<Guid>> roleManager,
          IMapper mapper,
          ITokenService tokenService,
          IPublisher<MailData> mailPublisher
          )
        {
            this._userManager = userManager;
            this.tokenService = tokenService;
            this._roleManager = roleManager;
            _mapper = mapper;
            this._mailPublisher = mailPublisher;
        }

        public async Task<UserDTO> GetUserInfoAsync(string? userId, CancellationToken token = default)
        {
            if (userId == null)
            {
                throw new UserNotFoundException();
            }

            token.ThrowIfCancellationRequested();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            UserDTO userDTO = _mapper.Map<UserDTO>(user);

            return userDTO;
        }

        public async Task<string> LoginAsync(LoginModel model, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(model.Email) ??
                throw new UserNotFoundException();

            var credetialsResult = await _userManager.CheckPasswordAsync(
                user, model.Password);
            if (!credetialsResult)
            {
                throw new InvalidCredentialsException();
            }

            List<Claim> additionalClaims = new List<Claim>();

            string? rolename = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            if (rolename != null)
            {
                additionalClaims.Add(new Claim(ClaimTypes.Role, rolename));
            }

            cancellationToken.ThrowIfCancellationRequested();
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

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new InvalidPasswordRequirementsExceptions(result.Errors);
            }

            result = await _userManager.AddToRoleAsync(user, nameof(Roles.User));
            if (!result.Succeeded)
            {
                throw new AddingToRoleException(result.Errors);
            }
        }

        public async Task SendEmailVerificationAsync(string? userId, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new UserNotFoundException();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var mailData = new MailData(
                  new List<string> { user.Email },
                  "RecordHub confirmation code",
                  "Confirm your email address\n" +
                  "Your confirmation code is below - enter it in your open browser window.\n" +
                  $"{token}");
            await _mailPublisher.PublishMessage(mailData);

        }

        public async Task VerifyEmailAsync(string token, string? userId, CancellationToken cancellationToken = default)
        {
            if (userId == null)
            {
                throw new UserNotFoundException();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            cancellationToken.ThrowIfCancellationRequested();

            var confirmationResult = await _userManager.ConfirmEmailAsync(
                user, token);

            if (confirmationResult.Errors.Any())
            {
                throw new ConfirmationEmailException(confirmationResult.Errors);
            }
        }
    }
}
