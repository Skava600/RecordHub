using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Exceptions;
using RecordHub.IdentityService.Core.Publishers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.IdentityService.Infrastructure.Services;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;
using RecordHub.Shared.Exceptions;
using RecordHub.Shared.MassTransit.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPublisher<MailData>> _mockMailPublisher;
        private readonly Mock<IAddressRepository> _mockAddressRepository;

        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            _mockMapper = new Mock<IMapper>();
            _mockTokenService = new Mock<ITokenService>();
            _mockMailPublisher = new Mock<IPublisher<MailData>>();
            _mockAddressRepository = new Mock<IAddressRepository>();

            _mockUserManager.Object.UserValidators.Add(new UserValidator<User>());
            _mockUserManager.Object.PasswordValidators.Add(new PasswordValidator<User>());

            _accountService = new AccountService(
                _mockUserManager.Object,
                _mockMapper.Object,
                _mockTokenService.Object,
                _mockMailPublisher.Object,
                _mockAddressRepository.Object
            );
        }

        [Fact]
        public async Task RegisterAsync_ValidModel_CreatesUserAndAddsToRole()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Name = "John",
                Surname = "Doe",
                PhoneNumber = "123456789",
                Password = "P@ssw0rd"
            };

            var creatingUserResult = IdentityResult.Success;
            var addingToRoleResult = IdentityResult.Success;

            _mockUserManager
                .Setup(m => m
                .CreateAsync(
                    It.IsAny<User>(),
                    model.Password))
                .ReturnsAsync(creatingUserResult);

            _mockUserManager
                .Setup(m => m
                .AddToRoleAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .ReturnsAsync(addingToRoleResult);

            // Act
            await _accountService.RegisterAsync(model);

            // Assert
            _mockUserManager
                .Verify(m => m
                .CreateAsync(
                    It.IsAny<User>(),
                    model.Password),
                Times.Once);

            _mockUserManager
                .Verify(m => m
                .AddToRoleAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_InvalidPassword_ThrowsInvalidPasswordRequirementsException()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Name = "John",
                Surname = "Doe",
                PhoneNumber = "123456789",
                Password = "123456"
            };

            var creatingUserResult = IdentityResult.Failed(new IdentityError { Description = "Invalid password" });

            _mockUserManager
                .Setup(m => m
                .CreateAsync(
                    It.IsAny<User>(),
                    model.Password))
                .ReturnsAsync(creatingUserResult);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidPasswordRequirementsExceptions>(() => _accountService.RegisterAsync(model));
        }

        [Fact]
        public async Task RegisterAsync_AddingToRoleFails_ThrowsAddingToRoleException()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Name = "John",
                Surname = "Doe",
                PhoneNumber = "123456789",
                Password = "P@ssw0rd"
            };

            var creatingUserResult = IdentityResult.Success;
            var addingToRoleResult = IdentityResult.Failed(new IdentityError { Description = "Failed to add user to role" });

            _mockUserManager
                .Setup(m => m
                .CreateAsync(
                    It.IsAny<User>(),
                    model.Password))
                .ReturnsAsync(creatingUserResult);

            // Mock roles and setup mock UserManager
            var roles = new List<string> { nameof(Roles.Admin) };

            var mockRoleManager = new Mock<RoleManager<IdentityRole>>
                (Mock.Of<IRoleStore<IdentityRole>>(),
                null,
                null,
                null,
                null);
            mockRoleManager
                .Setup(rm => rm.Roles)
                .Returns(roles.Select(r => new IdentityRole(r)).AsQueryable());

            _mockUserManager
                .Setup(m => m
                .AddToRoleAsync(
                    It.Is<User>(u => u.Email.Equals(model.Email)),
                    It.IsAny<string>()))
                .ReturnsAsync((User user, string role) =>
                {
                    if (!roles.Contains(role))
                    {
                        return IdentityResult.Failed(new IdentityError { Description = "Role does not exist." });
                    }

                    return addingToRoleResult;
                });

            // Act and Assert
            await Assert.ThrowsAsync<AddingToRoleException>(() => _accountService.RegisterAsync(model));
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var model = new LoginModel
            {
                UserName = "testuser",
                Password = "P@ssw0rd"
            };

            var user = new User
            {
                UserName = model.UserName
            };

            var role = nameof(Roles.User);
            _mockUserManager
                .Setup(m => m.GetRolesAsync(user))
                .ReturnsAsync(new List<string> { role });

            _mockUserManager
                .Setup(m => m.FindByNameAsync(model.UserName))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m.CheckPasswordAsync(user, model.Password))
                .ReturnsAsync(true);

            var token = "dummytoken";
            _mockTokenService
                .Setup(m => m
                .GenerateJwtToken(
                    user,
                    It.IsAny<List<Claim>>()))
                .Returns(token);

            // Act
            var result = await _accountService.LoginAsync(model);

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task LoginAsync_InvalidUsername_ThrowsUserNotFoundException()
        {
            // Arrange
            var model = new LoginModel
            {
                UserName = "nonexistentuser",
                Password = "P@ssw0rd"
            };

            _mockUserManager
                .Setup(m => m
                .FindByNameAsync(model.UserName))
                .ReturnsAsync((User)null);

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.LoginAsync(model));
        }

        [Fact]
        public async Task LoginAsync_InvalidCredentials_ThrowsInvalidCredentialsException()
        {
            // Arrange
            var model = new LoginModel
            {
                UserName = "testuser",
                Password = "InvalidPassword"
            };

            var user = new User
            {
                UserName = model.UserName
            };

            _mockUserManager
                .Setup(m => m
                .FindByNameAsync(model.UserName))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m
                .CheckPasswordAsync(user, model.Password))
                .ReturnsAsync(false);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _accountService.LoginAsync(model));
        }

        [Fact]
        public async Task LoginAsync_RoleExists_AddsRoleClaim()
        {
            // Arrange
            var model = new LoginModel
            {
                UserName = "testuser",
                Password = "P@ssw0rd"
            };

            var user = new User
            {
                UserName = model.UserName
            };

            _mockUserManager
                .Setup(m => m
                .FindByNameAsync(model.UserName))
                .ReturnsAsync(user);

            _mockUserManager.Setup(m => m
                .CheckPasswordAsync(user, model.Password))
                .ReturnsAsync(true);

            var role = nameof(Roles.User);
            _mockUserManager
                .Setup(m => m
                .GetRolesAsync(user))
                .ReturnsAsync(new List<string> { role });

            var token = "dummytoken";
            _mockTokenService
                .Setup(m => m
                .GenerateJwtToken(
                    user,
                    It.IsAny<IEnumerable<Claim>>()))
              .Callback<User, IEnumerable<Claim>>((_, claims) =>
              {
                  var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                  Assert.NotNull(roleClaim);
                  Assert.Equal(role, roleClaim.Value);
              })
              .Returns(token);

            // Act
            var result = await _accountService.LoginAsync(model);

            // Assert
            Assert.Equal(token, result);
        }

        [Fact]
        public async Task GetUserInfoAsync_ValidUserId_ReturnsUserDTO()
        {
            // Arrange
            Guid userId = Guid.Empty;

            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                PhoneNumber = "123456789",
                Name = "John",
                Surname = "Doe",
                Addresses = new List<Address>()
            };

            var userDTO = new UserDTO
            {
                Email = "test@example.com",
                PhoneNumber = "123456789",
                Name = "John",
                Surname = "Doe",
                Addresses = new List<Address>()
            };

            var addresses = new List<Address>
            {
                new Address
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    State = "State 1",
                    City = "City 1",
                    Street = "Street 1",
                    HouseNumber = "1",
                    Korpus = "K1",
                    Appartment = "A1",
                    Postcode = "12345"
                },
                new Address
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    State = "State 2",
                    City = "City 2",
                    Street = "Street 2",
                    HouseNumber = "2",
                    Korpus = "K2",
                    Appartment = "A2",
                    Postcode = "23456"
                },
                // Add more addresses as needed
            };

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockMapper
                .Setup(m => m.Map<UserDTO>(user))
                .Returns(userDTO);

            _mockAddressRepository
                .Setup(m => m.GetAddressesByUserId(userId))
                .Returns(addresses);

            // Act
            var result = await _accountService.GetUserInfoAsync(userId.ToString());

            // Assert
            Assert.Equal(userDTO, result);
        }

        [Fact]
        public async Task GetUserInfoAsync_NullUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            string userId = null;

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.GetUserInfoAsync(userId));
        }

        [Fact]
        public async Task GetUserInfoAsync_InvalidUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            var userId = "invalidUserId";

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.GetUserInfoAsync(userId));
        }

        [Fact]
        public async Task VerifyEmailAsync_ValidTokenAndUserId_ConfirmsEmail()
        {
            // Arrange
            var token = "validToken";
            Guid userId = Guid.Empty;

            var user = new User
            {
                Id = userId
            };

            var confirmationResult = IdentityResult.Success;

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m
                .ConfirmEmailAsync(user, token))
                .ReturnsAsync(confirmationResult);

            // Act
            await _accountService.VerifyEmailAsync(token, userId.ToString());

            // Assert
            _mockUserManager
                .Verify(m => m
                .ConfirmEmailAsync(user, token),
                Times.Once);
        }

        [Fact]
        public async Task VerifyEmailAsync_NullUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            var token = "validToken";
            string userId = null;

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.VerifyEmailAsync(token, userId));
        }

        [Fact]
        public async Task VerifyEmailAsync_InvalidUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            var token = "validToken";
            var userId = "invalidUserId";

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.VerifyEmailAsync(token, userId));
        }

        [Fact]
        public async Task VerifyEmailAsync_ConfirmationFails_ThrowsConfirmationEmailException()
        {
            // Arrange
            var token = "validToken";
            Guid userId = Guid.Empty;

            var user = new User
            {
                Id = userId
            };

            var confirmationResult = IdentityResult.Failed(new IdentityError { Description = "Email confirmation failed." });

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockUserManager.Setup(m => m.ConfirmEmailAsync(user, token))
                .ReturnsAsync(confirmationResult);

            // Act and Assert
            await Assert.ThrowsAsync<ConfirmationEmailException>(() => _accountService.VerifyEmailAsync(token, userId.ToString()));
        }

        [Fact]
        public async Task SendEmailVerificationAsync_InvalidUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            var userId = "invalidUserId";

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.SendEmailVerificationAsync(userId));
        }

        [Fact]
        public async Task SendEmailVerificationAsync_NullUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            string userId = null;

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _accountService.SendEmailVerificationAsync(userId));
        }

        [Fact]
        public async Task SendEmailVerificationAsync_ValidUserId_SendsEmailVerification()
        {
            // Arrange
            Guid userId = Guid.Empty;

            var user = new User
            {
                Id = userId,
                Email = "test@example.com"
            };

            var token = "validToken";
            var mailData = new MailData(
                new List<string> { user.Email },
                "RecordHub confirmation code",
                "Confirm your email address\n" +
                "Your confirmation code is below - enter it in your open browser window.\n" +
                $"{token}");

            _mockUserManager
                .Setup(m => m
                .FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            _mockUserManager
                .Setup(m => m
                .GenerateEmailConfirmationTokenAsync(user))
                .ReturnsAsync(token);

            _mockMailPublisher
                .Setup(m => m
                .PublishMessage(mailData))
                .Returns(Task.CompletedTask);

            // Act
            await _accountService.SendEmailVerificationAsync(userId.ToString());

            // Assert
            _mockMailPublisher
                .Verify(m => m
                .PublishMessage(It.IsAny<MailData>()),
                Times.Once);
        }
    }
}
