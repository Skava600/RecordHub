using FluentAssertions;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Tests.Generators;
using RecordHub.IdentityService.Tests.IntegrationTests.Helpers;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.IdentityService.Tests.IntegrationTests
{
    public class AddressesControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly JsonSerializerOptions _jsonOptions;
        protected readonly HttpClient client;
        protected dynamic token;

        public AddressesControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            client = factory.CreateClient();

            token = new ExpandoObject();
            token.sub = factory.UserId;
            token.role = new[] { "sub_role", "Admin" };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task AddAddressAsync_ValidData_ReturnsOk()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var addressModel = new AddressGenerator().GenerateModel();

            var content = new StringContent(JsonSerializer.Serialize(addressModel), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/addresses", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var createdAddress = JsonSerializer.Deserialize<Address>(responseContent, _jsonOptions);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            createdAddress.Should().NotBeNull();
        }

        [Fact]
        public async Task AddAddressAsync_NonValidData_ReturnsBadRequest()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var addressModel = new AddressGenerator().GenerateModel();
            addressModel.Korpus = "not-valid-korpus";

            var content = new StringContent(JsonSerializer.Serialize(addressModel), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/addresses", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateAddressAsync_ValidData_ReturnsOk()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var addressId = TestData.AddressToUpdate.Id;
            var addressModel = new AddressGenerator().GenerateModel();

            var content = new StringContent(JsonSerializer.Serialize(addressModel), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"/api/addresses/{addressId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var updatedAddress = JsonSerializer.Deserialize<Address>(responseContent, _jsonOptions);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            updatedAddress.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAddressAsync_NonExistingAddress_ReturnsOk()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var addressId = Guid.Empty;
            var addressModel = new AddressGenerator().GenerateModel();

            var content = new StringContent(JsonSerializer.Serialize(addressModel), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PutAsync($"/api/addresses/{addressId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAddressAsync_ValidData_ReturnsOk()
        {
            // Arrange
            client.SetFakeBearerToken((object)token);
            var addressId = TestData.AddressToDelete.Id;

            // Act
            var response = await client.DeleteAsync($"/api/addresses/{addressId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
