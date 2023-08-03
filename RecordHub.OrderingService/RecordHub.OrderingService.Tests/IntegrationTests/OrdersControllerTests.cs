using FluentAssertions;
using RecordHub.OrderingService.Domain.Entities;
using RecordHub.OrderingService.Tests.IntegrationTests.Helpers;
using RecordHub.Shared.Enums;
using System.Dynamic;
using System.Net;
using System.Text.Json;

namespace RecordHub.OrderingService.Tests.IntegrationTests
{
    public class OrdersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected readonly dynamic _token;

        public OrdersControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _client = factory.CreateClient();

            _token = new ExpandoObject();
            _token.sub = _factory.UserId;
            _token.role = new[] { "sub_role", "Admin" };
        }

        [Fact]
        public async Task ChangeOrderState_NotValidOrderId_ReturnsInternalServerError()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);
            var orderId = Guid.Empty;
            var state = StatesEnum.Completed;

            // Act
            var response = await _client.PutAsync($"/api/orders/{orderId}/{state}", null);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task ChangeOrderState_ValidData_ReturnsOk()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);
            var orderId = Guid.Parse("47a3aa1c-21df-46c3-8835-f3ab33dcfcd8");
            var state = StatesEnum.Completed;

            // Act
            var response = await _client.PutAsync($"/api/orders/{orderId}/{state}", null);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetUsersOrders_AdminUser_MatchingUserId_ReturnsOkWithOrders()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);
            var userId = _factory.UserId;

            // Act
            var response = await _client.GetAsync($"/api/orders/{userId}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<Order>>(content);

            orders
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetUsersOrders_NonAdminUser_MatchingUserId_ReturnsOkWithOrders()
        {
            // Arrange
            dynamic token = new ExpandoObject();
            token.sub = _factory.UserId;
            token.role = new[] { "sub_role" };
            _client.SetFakeBearerToken((object)token);
            var userId = _factory.UserId;

            // Act
            var response = await _client.GetAsync($"/api/orders/{userId}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<Order>>(content);

            orders
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetUsersOrders_NonAdminUser_NonMatchingUserId_ReturnsInternalServerError()
        {
            // Arrange
            dynamic token = new ExpandoObject();
            token.sub = Guid.NewGuid().ToString();
            token.role = new[] { "sub_role" };
            _client.SetFakeBearerToken((object)token);
            var userId = _factory.UserId;

            // Act
            var response = await _client.GetAsync($"/api/orders/{userId}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GetUsersOrders_AdminUser_NonMatchingUserId_ReturnsOkWithOrders()
        {
            // Arrange
            dynamic token = new ExpandoObject();
            token.sub = Guid.NewGuid().ToString();
            token.role = new[] { "sub_role", "Admin" };
            _client.SetFakeBearerToken((object)token);
            var userId = _factory.UserId;

            // Act
            var response = await _client.GetAsync($"/api/orders/{userId}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<Order>>(content);

            orders
                .Should()
                .NotBeNullOrEmpty();
        }
    }
}
