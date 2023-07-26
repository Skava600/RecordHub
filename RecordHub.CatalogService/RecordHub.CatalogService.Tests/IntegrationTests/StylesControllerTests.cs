﻿using FluentAssertions;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Tests.IntegrationTests.Helpers;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.CatalogService.Tests.IntegrationTests
{
    public class StylesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient client;
        protected dynamic token;

        public StylesControllerTests(CustomWebApplicationFactory<Program> factory)
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

            var model = TestData.StyleModelForCreating;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Styles", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateAsync_InvalidModel_ReturnsInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var model = TestData.InvalidStyleModel;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Styles", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var styleToUpdate = TestData.Styles[0];
            var model = new StyleModel
            {
                Slug = styleToUpdate.Slug,
                Name = "New Style Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Styles/{styleToUpdate.Id}", content);
            var allStylesResponse = await client.GetAsync($"api/Styles");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            allStylesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var stylesContent = await allStylesResponse.Content.ReadAsStringAsync();
            var styleDTOs = JsonSerializer.Deserialize<List<StyleDTO>>(stylesContent);
            styleDTOs.Should().NotBeNull();
            styleDTOs.Find(s => s.Name.Equals(model.Name)).Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_NotExistingStyle_ReturnsInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var id = Guid.Empty;
            var model = new StyleModel
            {
                Slug = "not-existing-style",
                Name = "New Style Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Styles/{id}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkResponse()
        {
            // Act
            var response = await client.GetAsync("api/Styles");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var styles = JsonSerializer.Deserialize<List<StyleDTO>>(content);
            styles.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var styleToDelete = TestData.Styles[4];
            var idToDelete = styleToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"api/Styles/{idToDelete}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
