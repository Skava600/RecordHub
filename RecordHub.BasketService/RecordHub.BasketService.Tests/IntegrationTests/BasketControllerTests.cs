using FluentAssertions;
using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;
using RecordHub.BasketService.Tests.Generators;
using RecordHub.BasketService.Tests.IntegrationTests.Helpers;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.BasketService.Tests.IntegrationTests
{
    public class BasketControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient client;
        protected dynamic token;

        public BasketControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            client = factory.CreateClient();

            token = new ExpandoObject();
            token.sub = _factory.UserId;
            token.role = new[] { "sub_role", "Admin" };
        }

        [Fact]
        public async Task GetBasketAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            // Act
            var response = await client.GetAsync("api/Basket");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var basket = JsonSerializer.Deserialize<Basket>(content);
            basket.Should().NotBeNull();
            basket.UserName.Should().BeEquivalentTo(_factory.UserId);
        }

        [Fact]
        public async Task UpdateBasketAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var cartItem = new BasketItemModel
            {
                ProductId = Guid.NewGuid().ToString(),
                Quantity = 10
            };

            var json = JsonSerializer.Serialize(cartItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Basket", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task BasketCheckoutAsync_ReturnsAcceptedResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var model = BasketCheckoutModelGenerator.GenerateModel();
            model.UserId = _factory.UserId;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Basket/checkout", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Accepted);
        }

        [Fact]
        public async Task ClearBasketAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            // Act
            var response = await client.DeleteAsync("api/Basket");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
