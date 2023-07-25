using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Tests.IntegrationTests.Helpers;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.CatalogService.Tests.IntegrationTests
{
    public class ArtistsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient client;
        protected dynamic token;

        public ArtistsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            client = factory.CreateClient();

            token = new ExpandoObject();
            token.sub = Guid.NewGuid();
            token.role = new[] { "sub_role", "Admin" };
        }

        [Fact]
        public async Task CreateAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var model = new ArtistModel
            {
                Slug = "ac-dc",
                Name = "AC/DC"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Artists", content);

            // Assert
            response.EnsureSuccessStatusCode();
            // Add more assertions as needed for the specific scenario
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var model = new ArtistModel
            {
                Slug = "rammstein",
                Name = "new name"
            };

            var artistResponse = await client.GetAsync($"api/Artists/{model.Slug}");
            var artistJson = await artistResponse.Content.ReadAsStringAsync();
            var artist = JsonSerializer.Deserialize<ArtistDTO>(artistJson);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // Act
            var response = await client.PutAsync($"api/Artists/{artist.Id}", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

    }
}
