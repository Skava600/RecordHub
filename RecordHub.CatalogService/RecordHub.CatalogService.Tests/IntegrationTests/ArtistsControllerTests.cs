using FluentAssertions;
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
        private readonly JsonSerializerOptions jsonOptions;

        public ArtistsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            client = factory.CreateClient();

            token = new ExpandoObject();
            token.sub = Guid.NewGuid();
            token.role = new[] { "sub_role", "Admin" };
            jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
        }

        [Fact]
        public async Task CreateAsync_ValidModel_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var model = TestData.ArtistModelForCreating;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Artists", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UpdateAsync_ExistingId_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var artistToUpdate = TestData.Artists.First(a => a.Slug.Equals("rammstein"));
            var model = new ArtistModel
            {
                Slug = artistToUpdate.Slug,
                Name = "new name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Artists/{artistToUpdate.Id}", content);
            var updatedArtistResponse = await client.GetAsync($"api/Artists/{artistToUpdate.Slug}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            updatedArtistResponse.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            var artistContent = await updatedArtistResponse.Content.ReadAsStringAsync();
            var artistDto = JsonSerializer.Deserialize<ArtistDTO>(artistContent, jsonOptions);
            artistDto
                .Should()
                .NotBeNull();

            artistDto.Name
                .Should()
                .Be(model.Name);
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsNoContent()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var artistToDelete = TestData.Artists.First(a => a.Slug.Equals("delete-artist"));
            var idToDelete = artistToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"api/Artists/{idToDelete}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetBySlug_ExistingSlug_ReturnsOkResponse()
        {
            // Arrange
            var artistToGet = TestData.Artists[1];
            var slug = artistToGet.Slug;

            // Act
            var response = await client.GetAsync($"api/Artists/{slug}");

            // Assert
            response.StatusCode
                 .Should()
                 .Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var artistDto = JsonSerializer.Deserialize<ArtistDTO>(content, jsonOptions);
            artistDto
                .Should()
                .NotBeNull();

            artistDto.Name
                .Should()
                .Be(artistToGet.Name);
        }

        [Fact]
        public async Task GetBySlug_NotExistingSlug_ReturnsNotFoundResponse()
        {
            // Arrange
            var slug = "not-existing-slug";

            // Act
            var response = await client.GetAsync($"api/Artists/{slug}");

            // Assert
            response.StatusCode
                 .Should()
                 .Be(HttpStatusCode.NotFound);
        }
    }
}
