using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Api.Controllers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests
{
    public class AddressesControllerTests
    {

        [Fact]
        public async Task AddAddressAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "userId";
            var model = new AddressModel
            {
                State = "State",
                City = "City",
                Street = "Street",
                HouseNumber = "HouseNumber",
                Korpus = "Korpus",
                Appartment = "Appartment",
                Postcode = "Postcode"
            };

            var addressServiceMock = new Mock<IAddressService>();
            addressServiceMock
                .Setup(m => m.AddAsync(userId, model, cancellationToken))
                .Returns(Task.CompletedTask);

            var controller = new AddressesController(addressServiceMock.Object);
            controller.ControllerContext = GetMockControllerContextWithUser(userId);

            // Act
            var result = await controller.AddAddressAsync(model, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task UpdateAddressAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var id = Guid.NewGuid();
            var model = new AddressModel
            {
                State = "State",
                City = "City",
                Street = "Street",
                HouseNumber = "HouseNumber",
                Korpus = "Korpus",
                Appartment = "Appartment",
                Postcode = "Postcode"
            };

            var addressServiceMock = new Mock<IAddressService>();
            addressServiceMock
                .Setup(m => m.UpdateAsync(id, model, cancellationToken))
                .Returns(Task.CompletedTask);

            var controller = new AddressesController(addressServiceMock.Object);

            // Act
            var result = await controller.UpdateAddressAsync(id, model, cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteAddressAsync_AuthorizedUser_ReturnsOkResult()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var id = Guid.NewGuid();

            var addressServiceMock = new Mock<IAddressService>();
            addressServiceMock
                .Setup(m => m.DeleteAsync(id, cancellationToken))
                .Returns(Task.CompletedTask);

            var controller = new AddressesController(addressServiceMock.Object);

            // Act
            var result = await controller.DeleteAddressAsync(id, cancellationToken);

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

