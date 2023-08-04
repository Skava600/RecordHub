using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Api.Controllers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Tests.Generators;
using RecordHub.IdentityService.Tests.Setups;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests.UnitTests.Controllers
{
    public class AddressesControllerTests
    {
        private AddressGenerator _addressGenerator;
        private Mock<IAddressService> _addressServiceMock;

        public AddressesControllerTests()
        {
            _addressServiceMock = new Mock<IAddressService>();
            _addressGenerator = new AddressGenerator();
        }

        [Fact]
        public async Task AddAddressAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "userId";
            var model = _addressGenerator.GenerateModel();

            _addressServiceMock.SetupAddAsync();

            var controller = new AddressesController(_addressServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId);

            // Act
            var result = await controller.AddAddressAsync(model, cancellationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task UpdateAddressAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var id = Guid.NewGuid();
            var model = _addressGenerator.GenerateModel();

            _addressServiceMock.SetupUpdateAsync();

            var controller = new AddressesController(_addressServiceMock.Object);

            controller.ControllerContext = GetMockControllerContextWithUser(string.Empty);
            // Act
            var result = await controller.UpdateAddressAsync(id, model, cancellationToken);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteAddressAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var id = Guid.NewGuid();

            _addressServiceMock.SetupDeleteAsync();

            var controller = new AddressesController(_addressServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(string.Empty);

            // Act
            var result = await controller.DeleteAddressAsync(id, cancellationToken);

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

