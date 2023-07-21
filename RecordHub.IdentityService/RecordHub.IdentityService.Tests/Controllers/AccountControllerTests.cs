using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Api.Controllers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.IdentityService.Tests.Generators;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests.Controllers
{
    public class AccountControllerTests
    {
        private Mock<IAccountService> _accountServiceMock;
        private UserGenerator _userGenerator;


        public AccountControllerTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _userGenerator = new UserGenerator();
        }

        [Fact]
        public async Task LoginAsync_ValidModel_ReturnsOkResultWithToken()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = new LoginModel
            {
                UserName = "testuser",
                Password = "password123"
            };

            var expectedToken = "validToken";
            _accountServiceMock.SetupLoginAsync(expectedToken);

            var controller = new AuthController(_accountServiceMock.Object);

            // Act
            var result = await controller.LoginAsync(model, cancellationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().Be(expectedToken);
        }

        [Fact]
        public async Task RegisterAsync_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = _userGenerator.GenerateRegisterModel();

            _accountServiceMock.SetupRegisterAsync();

            var controller = new AuthController(_accountServiceMock.Object);

            // Act
            var result = await controller.RegisterAsync(model, cancellationToken);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task UserInfo_AuthorizedUser_ReturnsOkResultWithUserInfo()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = Guid.Empty;
            var expectedUser = _userGenerator.GenerateUserDTO();

            _accountServiceMock.SetupGetUserInfoAsync(expectedUser);

            var controller = new AuthController(_accountServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId.ToString());

            // Act
            var result = await controller.UserInfo(cancellationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task VerifyEmailAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "userId";
            var token = "validToken";

            _accountServiceMock.SetupVerifyEmailAsync();

            var controller = new AuthController(_accountServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId);

            // Act
            var result = await controller.VerifyEmailAsync(token, cancellationToken);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task SendVerificationEmail_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "userId";

            _accountServiceMock.SetupSendEmailVerificationAsync();

            var controller = new AuthController(_accountServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId);

            // Act
            var result = await controller.SendVerificationEmail(cancellationToken);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        private static ControllerContext GetMockControllerContextWithUser(string userId)
        {
            var httpContextMock = new Mock<HttpContext>();
            var claimsPrincipalMock = new Mock<ClaimsPrincipal>();

            claimsPrincipalMock
                .Setup(c => c.FindFirst(It.IsAny<string>()))
                .Returns(new Claim(ClaimTypes.NameIdentifier, userId));

            httpContextMock.SetupGet(c => c.User).Returns(claimsPrincipalMock.Object);

            var controllerContext = new ControllerContext
            {
                HttpContext = httpContextMock.Object
            };

            return controllerContext;
        }
    }
}
