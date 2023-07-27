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
    public class CountriesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        protected HttpClient client;
        protected dynamic token;

        public CountriesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            client = factory.CreateClient();

            token = new ExpandoObject();
            token.sub = Guid.NewGuid();
            token.role = new[] { "sub_role", "Admin" };
        }

        [Fact]
        public async Task CreateAsync_ValidModel_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var model = TestData.CountryModelForCreating;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Countries", content);

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

            var model = TestData.InvalidCountryModel;

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("api/Countries", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task UpdateAsync_ExistingId_ReturnsOkResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var countryToUpdate = TestData.Countries[0];
            var model = new CountryModel
            {
                Slug = countryToUpdate.Slug,
                Name = "New Country Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Countries/{countryToUpdate.Id}", content);
            var allCountriesResponse = await client.GetAsync($"api/Countries");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            allCountriesResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var countriesContent = await allCountriesResponse.Content.ReadAsStringAsync();
            var countryDTOs = JsonSerializer.Deserialize<List<CountryDTO>>(countriesContent);
            countryDTOs
                .Should()
                .NotBeNull();

            countryDTOs
                .Find(c => c.Name.Equals(model.Name))
                .Should()
                .NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_NotExistingId_ReturnsInternalErrorResponse()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var id = Guid.Empty;
            var model = new CountryModel
            {
                Slug = "not-existing-country",
                Name = "New Country Name"
            };

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"api/Countries/{id}", content);

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async Task GetAllAsync_ExistingCountries_ReturnsOkResponse()
        {
            // Act
            var response = await client.GetAsync("api/Countries");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<List<CountryDTO>>(content);
            countries
                .Should()
                .NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsNoContent()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);

            var countryToDelete = TestData.Countries[4];
            var idToDelete = countryToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"api/Countries/{idToDelete}");

            // Assert
            response.StatusCode
                .Should()
                .Be(HttpStatusCode.NoContent);
        }
    }
}