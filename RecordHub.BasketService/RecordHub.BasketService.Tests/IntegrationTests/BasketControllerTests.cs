using FluentAssertions;
using RecordHub.BasketService.Domain.Models;
using RecordHub.BasketService.Tests.Generators;
using RecordHub.BasketService.Tests.IntegrationTests.DTOs;
using RecordHub.BasketService.Tests.IntegrationTests.Helpers;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit.Priority;

namespace RecordHub.BasketService.Tests.IntegrationTests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class BasketControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient _client;
        protected dynamic _token;

        public BasketControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            _token = new ExpandoObject();
            _token.sub = _factory.UserId;
            _token.role = new[] { "sub_role", "Admin" };
        }

        [Fact, Priority(-20)]
        public async Task GetBasketAsync_ReturnsOkResponse()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);

            // Act
            var response = await _client.GetAsync("api/Basket");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var basket = JsonSerializer.Deserialize<BasketDTO>(content);
            basket
                .Should()
                .NotBeNull();

            basket.Items
                .Should()
                .NotBeEmpty();

            basket.UserName
                .Should()
                .BeEquivalentTo(_factory.UserId);
        }

        [Fact, Priority(-10)]
        public async Task UpdateBasketAsync_ReturnsOkResponse()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);

            var cartItem = new BasketItemModel
            {
                ProductId = Guid.NewGuid().ToString(),
                Quantity = 10
            };

            var json = JsonSerializer.Serialize(cartItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("api/Basket", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var basketContent = await response.Content.ReadAsStringAsync();
            var basket = JsonSerializer.Deserialize<BasketDTO>(basketContent);

            basket
                .Should()
                .NotBeNull();

            basket.Items
                .Should()
                .Contain(item => item
                    .ProductId.Equals(cartItem.ProductId) && item
                    .Quantity.Equals(cartItem.Quantity));

            basket.UserName
                .Should()
                .BeEquivalentTo(_factory.UserId);
        }

        [Fact, Priority(10)]
        public async Task BasketCheckoutAsync_ReturnsAcceptedResponse()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);

            var model = BasketCheckoutModelGenerator.GenerateModel();
            model.UserId = _factory.UserId;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("api/Basket/checkout", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.Accepted);
        }

        [Fact, Priority(-15)]
        public async Task DeleteItemAsync_ReturnsOkResponse()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);

            var productIdToDelete = Guid.NewGuid().ToString();

            var basketItem = new BasketItemModel
            {
                ProductId = productIdToDelete,
                Quantity = 1
            };

            var json = JsonSerializer.Serialize(basketItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var addToBasketResponse = await _client.PostAsync("api/Basket", content);
            addToBasketResponse.EnsureSuccessStatusCode();

            // Act
            var response = await _client.DeleteAsync($"api/Basket/{productIdToDelete}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Check if the returned basket matches the expected structure
            var responseBody = await response.Content.ReadAsStringAsync();
            var basket = JsonSerializer.Deserialize<BasketDTO>(responseBody);

            basket
                .Should()
                .NotBeNull();

            basket.UserName
                .Should()
                .Be(_factory.UserId);

            basket.Items
                .Should()
                .NotContain(item => item.ProductId.Equals(productIdToDelete));
        }

        [Fact, Priority(20)]
        public async Task ClearBasketAsync_ReturnsOkResponse()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);

            // Act
            var response = await _client.DeleteAsync("api/Basket");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }
    }
}
