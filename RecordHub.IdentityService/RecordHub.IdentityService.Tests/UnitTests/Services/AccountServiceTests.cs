using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.Exceptions;
using RecordHub.IdentityService.Core.Publishers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.IdentityService.Infrastructure.Services;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;
using RecordHub.IdentityService.Tests.Generators;
using RecordHub.IdentityService.Tests.Setups;
using RecordHub.Shared.Exceptions;
using RecordHub.Shared.MassTransit.Models;

namespace RecordHub.IdentityService.Tests.UnitTests.Services
{
    public class AccountServiceTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IPublisher<MailData>> _mockMailPublisher;
        private readonly Mock<IAddressRepository> _mockAddressRepository;
        private readonly UserGenerator _userGenerator;
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _userGenerator = new UserGenerator();
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
            var model = _userGenerator.GenerateRegisterModel();

            _mockUserManager.SetupCreateAsync();

            _mockUserManager.SetupAddToRoleAsync();

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
            var model = _userGenerator.GenerateRegisterModel();

            var creatingUserResult = IdentityResult.Failed(new IdentityError { Description = "Invalid password" });

            _mockUserManager.SetupCreateAsync(creatingUserResult);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _accountService.RegisterAsync(model))
                .Should().ThrowAsync<InvalidPasswordRequirementsExceptions>();
        }

        [Fact]
        public async Task RegisterAsync_AddingToRoleFails_ThrowsAddingToRoleException()
        {
            // Arrange
            var model = _userGenerator.GenerateRegisterModel();

            var creatingUserResult = IdentityResult.Success;
            var addingToRoleResult = IdentityResult.Failed(new IdentityError { Description = "Failed to add user to role" });

            _mockUserManager.SetupCreateAsync(creatingUserResult);


            _mockUserManager.SetupAddToRoleAsync(addingToRoleResult);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _accountService.RegisterAsync(model))
                .Should().ThrowAsync<AddingToRoleException>();
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
            _mockUserManager.SetupGetRolesAsync(new List<string> { role });

            _mockUserManager.SetupFindByNameAsync(user);

            _mockUserManager.SetupCheckPasswordAsync(true);

            var token = "dummytoken";
            _mockTokenService.SetupGenerateJwtToken(token);

            // Act
            var result = await _accountService.LoginAsync(model);

            // Assert
            result.Should().Be(token);
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

            _mockUserManager.SetupFindByNameAsync(null);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _accountService.LoginAsync(model))
                .Should().ThrowAsync<UserNotFoundException>();
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

            _mockUserManager.SetupFindByNameAsync(user);

            _mockUserManager.SetupCheckPasswordAsync(false);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _accountService.LoginAsync(model))
                .Should().ThrowAsync<InvalidCredentialsException>();
        }

        [Fact]
        public async Task GetUserInfoAsync_ValidUserId_ReturnsUserDTO()
        {
            // Arrange
            Guid userId = Guid.Empty;

            var user = new User();


            var userDTO = _userGenerator.GenerateUserDTO();

            _mockUserManager.SetupFindByIdAsync(user);

            _mockMapper.SetupMap(user, userDTO);

            _mockAddressRepository.SetupGetAddressesByUserId(userDTO.Addresses);

            // Act
            var result = await _accountService.GetUserInfoAsync(userId.ToString());

            // Assert
            userDTO.Should().BeEquivalentTo(result);
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

            _mockUserManager.SetupFindByIdAsync(null);

            // Act and Assert
            await FluentActions.Awaiting(() => _accountService.GetUserInfoAsync(userId.ToString()))
                .Should().ThrowAsync<UserNotFoundException>();
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

            _mockUserManager.SetupFindByIdAsync(user);

            _mockUserManager.SetupConfirmEmailAsync(confirmationResult);

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
            await FluentActions
                .Awaiting(() => _accountService.VerifyEmailAsync(token, userId))
                .Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task VerifyEmailAsync_InvalidUserId_ThrowsUserNotFoundException()
        {
            // Arrange
            var token = "validToken";
            var userId = "invalidUserId";

            _mockUserManager.SetupFindByIdAsync(null);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _accountService.VerifyEmailAsync(token, userId))
                .Should().ThrowAsync<UserNotFoundException>();
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

            _mockUserManager.SetupFindByIdAsync(user);

            _mockUserManager.SetupConfirmEmailAsync(confirmationResult);

            // Act and Assert
            await FluentActions
                .Awaiting(() => _accountService.VerifyEmailAsync(token, userId.ToString()))
                .Should().ThrowAsync<ConfirmationEmailException>();
        }

        [Theory]
        [InlineData("invalidUserId")]
        [InlineData(null)]
        public async Task SendEmailVerificationAsync_InvalidUserId_ThrowsUserNotFoundException(string userId)
        {
            // Arrange
            _mockUserManager.SetupFindByIdAsync(null);

            // Act and Assert
            await FluentActions.Awaiting(() => _accountService.SendEmailVerificationAsync(userId))
                .Should().ThrowAsync<UserNotFoundException>();
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

            _mockUserManager.SetupFindByIdAsync(user);

            _mockUserManager.SetupGenerateEmailConfirmationTokenAsync(token);

            _mockMailPublisher.SetupPublishMessage(Task.CompletedTask);

            // Act
            await FluentActions.Awaiting(() => _accountService.SendEmailVerificationAsync(userId.ToString()))
                    .Should().NotThrowAsync();

            // Assert
            _mockMailPublisher
                .Verify(m => m
                .PublishMessage(It.IsAny<MailData>()),
                Times.Once);
        }
    }
}
