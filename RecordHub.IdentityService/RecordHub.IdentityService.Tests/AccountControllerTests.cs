using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Api.Controllers;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests
{
    public class AuthControllerTests
    {
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
            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock
                .Setup(m => m.LoginAsync(model, cancellationToken))
                .ReturnsAsync(expectedToken);

            var controller = new AuthController(accountServiceMock.Object);

            // Act
            var result = await controller.LoginAsync(model, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualToken = Assert.IsType<string>(okResult.Value);
            Assert.Equal(expectedToken, actualToken);
        }

        [Fact]
        public async Task RegisterAsync_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                Name = "John",
                Surname = "Doe",
                PhoneNumber = "123456789"
            };

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock
                .Setup(m => m.RegisterAsync(model, cancellationToken))
                .Returns(Task.CompletedTask);

            var controller = new AuthController(accountServiceMock.Object);

            // Act
            var result = await controller.RegisterAsync(model, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UserInfo_AuthorizedUser_ReturnsOkResultWithUserInfo()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = Guid.Empty;
            var expectedUser = new UserDTO
            {
                Email = "test@example.com",
                PhoneNumber = "123456789",
                Name = "John",
                Surname = "Doe",
                Addresses = new List<Address>
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
                }
            }
            };

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock
                .Setup(m => m.GetUserInfoAsync(userId.ToString(), cancellationToken))
                .ReturnsAsync(expectedUser);

            var controller = new AuthController(accountServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId.ToString());

            // Act
            var result = await controller.UserInfo(cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualUser = Assert.IsType<UserDTO>(okResult.Value);
            Assert.Equal(expectedUser.Email, actualUser.Email);
            Assert.Equal(expectedUser.PhoneNumber, actualUser.PhoneNumber);
            Assert.Equal(expectedUser.Name, actualUser.Name);
            Assert.Equal(expectedUser.Surname, actualUser.Surname);
            Assert.Equal(expectedUser.Addresses, actualUser.Addresses);
        }

        [Fact]
        public async Task VerifyEmailAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "userId";
            var token = "validToken";

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock
                .Setup(m => m.VerifyEmailAsync(token, userId, cancellationToken))
                .Returns(Task.CompletedTask);

            var controller = new AuthController(accountServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId);

            // Act
            var result = await controller.VerifyEmailAsync(token, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task SendVerificationEmail_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "userId";

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock
                .Setup(m => m.SendEmailVerificationAsync(userId, cancellationToken))
                .Returns(Task.CompletedTask);

            var controller = new AuthController(accountServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId);

            // Act
            var result = await controller.SendVerificationEmail(cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
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
