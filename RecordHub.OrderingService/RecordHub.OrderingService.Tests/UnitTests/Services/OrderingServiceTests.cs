using AutoMapper;
using Bogus;
using FluentAssertions;
using RecordHub.OrderingService.Domain.Entities;
using RecordHub.OrderingService.Infrastructure.Data.Repositories;
using RecordHub.OrderingService.Tests.Generators;
using RecordHub.OrderingService.Tests.Setups;
using RecordHub.Shared.Enums;
using RecordHub.Shared.Exceptions;
using System.Security.Claims;

namespace RecordHub.OrderingService.Tests.UnitTests.Services
{
    public class OrderingServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOrderingRepository> _repositoryMock;
        private readonly Infrastructure.Services.OrderingService _orderingService;

        public OrderingServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _repositoryMock = new Mock<IOrderingRepository>();
            _orderingService = new Infrastructure.Services.OrderingService(
                _mapperMock.Object,
                _repositoryMock.Object);
        }

        [Fact]
        public async Task AddOrderAsync_ValidModel_AddsOrderAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = new BasketCheckoutMessageGenerator().Generate();

            var order = new Order
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailAddress = model.EmailAddress,
                UserId = model.UserId,
                TotalPrice = model.TotalPrice,
                Address = model.Address,
                Items = new OrderItemGenerator().GenerateBetween(2, 10)
            };

            _mapperMock.SetupMap(model, order);
            var validResult = new ValidationResult();

            _repositoryMock.SetupRepositoryMockAddAsync(cancellationToken);

            // Act
            await _orderingService.AddOrderAsync(model, cancellationToken);

            // Assert
            _repositoryMock
                .Verify(repo => repo
                .AddAsync(order, cancellationToken),
                Times.Once); // Verify AddAsync was called
        }


        [Fact]
        public async Task ChangeOrderStateAsync_OrderNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var orderId = Guid.NewGuid();

            _repositoryMock.SetupRepositoryMockGetAsync(null, cancellationToken);

            // Act and Assert
            await _orderingService
                .Invoking(async service => await service
                .ChangeOrderStateAsync(orderId, StatesEnum.Submitted, cancellationToken))
                .Should()
                .ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task ChangeOrderStateAsync_OrderFound_UpdatesOrderAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var orderId = Guid.NewGuid();
            var order = new OrderGenerator().Generate();

            _repositoryMock.SetupRepositoryMockGetAsync(order, cancellationToken);

            // Act
            await _orderingService.ChangeOrderStateAsync(orderId, StatesEnum.Completed, cancellationToken);

            // Assert
            order.State.Should().Be(StatesEnum.Completed);

            _repositoryMock
                .Verify(repo => repo
                .UpdateAsync(order, cancellationToken),
                Times.Once);
        }

        [Fact]
        public async Task GetUsersOrdersAsync_AdminUser_ReturnsUsersOrders()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "sample-user-id";

            var adminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            }));

            var orders = new OrderGenerator().GenerateBetween(2, 10);
            _repositoryMock.SetupRepositoryMockGetUsersOrdersAsync(orders, cancellationToken);

            // Act
            var result = await _orderingService.GetUsersOrdersAsync(userId, adminUser, cancellationToken);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(orders);
        }

        [Fact]
        public async Task GetUsersOrdersAsync_NonAdminUser_MatchingUserId_ReturnsUsersOrders()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "sample-user-id";

            var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "SomeRole")
            }));

            var orders = new OrderGenerator().GenerateBetween(2, 10);
            _repositoryMock.SetupRepositoryMockGetUsersOrdersAsync(orders, cancellationToken);

            // Act
            var result = await _orderingService.GetUsersOrdersAsync(userId, nonAdminUser, cancellationToken);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(orders);
        }

        [Fact]
        public async Task GetUsersOrdersAsync_NonAdminUser_NotMatchingUserId_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var userId = "sample-user-id";
            var nonMatchingUserId = "other-user-id";

            var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, "SomeRole")
            }));

            // Act and Assert
            await _orderingService
                .Invoking(async service => await service.GetUsersOrdersAsync(nonMatchingUserId, nonAdminUser, cancellationToken))
                .Should()
                .ThrowAsync<UnauthorizedAccessException>();
        }
    }
}
