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
    public class LabelsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient client;
        protected dynamic token;

        public LabelsControllerTests(CustomWebApplicationFactory<Program> factory)
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

            var model = TestData.LabelModelForCreating;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Labels", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateAsync_InvalidModel_ReturnsInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var model = TestData.InvalidLabelModel;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Labels", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var labelToUpdate = TestData.Labels.First(l => l.Slug.Equals("music-of-life"));
            var model = new LabelModel
            {
                Slug = labelToUpdate.Slug,
                Name = "New Label Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Labels/{labelToUpdate.Id}", content);
            var allLabelsResponse = await client.GetAsync($"api/Labels");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);

            allLabelsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var labelsContent = await allLabelsResponse.Content.ReadAsStringAsync();
            var labelDTOs = JsonSerializer.Deserialize<List<LabelDTO>>(labelsContent);
            labelDTOs
                .Should()
                .NotBeNull();

            labelDTOs
                .Find(l => l.Name.Equals(model.Name))
                .Should()
                .NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_NotExistingLabel_ReturnsInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var id = Guid.Empty;
            var model = new LabelModel
            {
                Slug = "not-existing-label",
                Name = "New Label Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Labels/{id}", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResponse()
        {
            // Act
            var response = await client.GetAsync("api/Labels");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var labels = JsonSerializer.Deserialize<List<LabelDTO>>(content);
            labels
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var labelToDelete = TestData.Labels.First(l => l.Slug.Equals("delete-label"));
            var idToDelete = labelToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"api/Labels/{idToDelete}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NoContent);
        }
    }
}
