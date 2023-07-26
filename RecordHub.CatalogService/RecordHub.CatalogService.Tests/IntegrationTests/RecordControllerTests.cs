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
    public class RecordControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient client;
        protected dynamic token;

        public RecordControllerTests(CustomWebApplicationFactory<Program> factory)
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

            var model = TestData.RecordModelForCreating;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Record", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateAsync_InvalidModel_ReturnInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var model = TestData.InvalidRecordModel;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Record", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var recordToUpdate = TestData.Records.First(r => r.Slug.Equals("record-one"));
            var model = new RecordModel
            {
                Name = "New Record Name",
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Record/{recordToUpdate.Id}", content);
            var recordResponse = await client.GetAsync($"api/Record/{recordToUpdate.Slug}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            recordResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var recordsContent = await recordResponse.Content.ReadAsStringAsync();
            var recordDTO = JsonSerializer.Deserialize<RecordDTO>(recordsContent);
            recordDTO
                .Should()
                .NotBeNull();

            recordDTO.Name
                .Should()
                .BeEquivalentTo(model.Name);
        }

        [Fact]
        public async Task UpdateAsync_NotExistingRecord_ReturnsInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var id = Guid.Empty;
            var model = new RecordModel
            {
                Slug = "not-existing-record",
                Name = "New Record Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Record/{id}", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var recordToDelete = TestData.Records.First(r => r.Slug.Equals("delete-record"));
            var idToDelete = recordToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"api/Record/{idToDelete}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task GetBySlugAsync_ReturnsOkResponse()
        {
            // Arrange
            var recordToGet = TestData.Records[1];
            var slug = recordToGet.Slug;

            // Act
            var response = await client.GetAsync($"api/Record/{slug}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var recordDto = JsonSerializer.Deserialize<RecordDTO>(content);
            recordDto
                .Should()
                .NotBeNull();

            recordDto.Name
                .Should()
                .Be(recordToGet.Name);
        }

        [Fact]
        public async Task GetBySlugAsync_NotExistingSlug_ReturnsNotFoundResponse()
        {
            // Arrange
            var slug = "not-existing-slug";

            // Act
            var response = await client.GetAsync($"api/Record/{slug}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetByPageAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var page = 1;
            var pageSize = 10;

            // Act
            var response = await client.GetAsync($"api/Record?page={page}&pageSize={pageSize}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var recordsDto = JsonSerializer.Deserialize<List<RecordDTO>>(content);
            recordsDto.Should().NotBeNull();
        }
    }
}