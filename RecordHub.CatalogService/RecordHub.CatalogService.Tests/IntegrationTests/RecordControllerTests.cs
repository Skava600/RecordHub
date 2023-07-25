using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Tests.IntegrationTests.Helpers;
using System.Text;
using System.Text.Json;

namespace RecordHub.CatalogService.Tests.IntegrationTests
{
    public class RecordControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public RecordControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResponse()
        {
            // Arrange
            var client = _factory.CreateClient();
            var model = new RecordModel
            {
                // Set the properties of the model as needed for the test
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Record", content);

            // Assert
            response.EnsureSuccessStatusCode();
            // Add more assertions as needed for the specific scenario
        }
    }
}
