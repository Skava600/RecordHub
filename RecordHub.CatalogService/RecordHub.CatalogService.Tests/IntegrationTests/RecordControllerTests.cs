using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Application.Services;
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
        private readonly JsonSerializerOptions jsonOptions;

        public RecordControllerTests(CustomWebApplicationFactory<Program> factory)
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

            var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IRecordCatalogService>();
            var cacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();

            var model = TestData.RecordModelForCreating;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Record", content);
            var createdRecord = await repo.GetBySlugAsync(model.Slug);
            var cachedRecord = await cacheService.GetAsync<RecordDTO>(model.Slug);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            createdRecord.Should().NotBeNull();
            createdRecord.Name.Should().Be(model.Name);
            cachedRecord.Should().BeEquivalentTo(createdRecord);
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
        public async Task UpdateAsync_ValidModel_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();

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
            var cached = await cacheService.GetAsync<RecordDTO>(recordToUpdate.Slug);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            recordResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var recordsContent = await recordResponse.Content.ReadAsStringAsync();
            var recordDTO = JsonSerializer.Deserialize<RecordDTO>(recordsContent, jsonOptions);
            recordDTO
                .Should()
                .NotBeNull();

            recordDTO.Name
                .Should()
                .BeEquivalentTo(model.Name);

            cached
                .Should()
                .BeEquivalentTo(recordDTO);
        }

        [Fact]
        public async Task UpdateAsync_NonExistingId_ReturnsInternalErrorResponse()
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
        public async Task DeleteAsync_ExistingId_ReturnsNoContent()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var recordToDelete = TestData.Records.First(r => r.Slug.Equals("delete-record"));
            var idToDelete = recordToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"api/Record/{idToDelete}");
            var deletedRecord = await repo.Records.GetByIdAsync(idToDelete);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NoContent);

            deletedRecord.Should().BeNull();
        }

        [Fact]
        public async Task GetBySlugAsync_ExistingSlug_ReturnsOkResponse()
        {
            // Arrange
            var recordToGet = TestData.Records[1];
            var slug = recordToGet.Slug;

            // Act
            var response = await client.GetAsync($"api/Record/{slug}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var content = await response.Content.ReadAsStringAsync();
            var recordDto = JsonSerializer.Deserialize<RecordDTO>(content, jsonOptions);
            recordDto
                .Should()
                .NotBeNull();

            recordDto.Name
                .Should()
                .Be(recordToGet.Name);
        }


        [Fact]
        public async Task GetBySlugAsync_ExistingSlugInCache_ReturnsOkResponse()
        {
            // Arrange
            var recordToGet = TestData.Records[1];
            var slug = recordToGet.Slug;

            var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var _cacheService = scope.ServiceProvider.GetRequiredService<IRedisCacheService>();
            var _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            var recordDtoToGet = _mapper.Map<RecordDTO>(recordToGet);
            await _cacheService.SetAsync(slug, recordDtoToGet);

            // Act
            var responseFromCache = await client.GetAsync($"api/Record/{slug}");

            responseFromCache.StatusCode.Should().Be(HttpStatusCode.OK);
            var contentFromCache = await responseFromCache.Content.ReadAsStringAsync();
            var recordDtoFromCache = JsonSerializer.Deserialize<RecordDTO>(contentFromCache, jsonOptions);

            // Assert

            recordDtoFromCache.Should().NotBeNull();

            recordDtoFromCache.Should().BeEquivalentTo(recordDtoToGet);
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
        public async Task GetByPageAsync_ValidRequest_ReturnsOkResponse()
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
            var recordsDto = JsonSerializer.Deserialize<List<RecordDTO>>(content, jsonOptions);
            recordsDto.Should().NotBeNull();
        }
    }
}