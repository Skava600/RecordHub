using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.IdentityService.Tests.Generators;
using RecordHub.IdentityService.Tests.IntegrationTests.Helpers;
using System.Dynamic;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.IdentityService.Tests.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly dynamic _token;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _factory = factory;

            _token = new ExpandoObject();
            _token.sub = factory.UserId;
            _token.role = new[] { "sub_role", "Admin" };
        }

        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsJwtToken()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserName = "test",
                Password = "123456aA."
            };

            var scope = _factory.Services.GetRequiredService<IServiceProvider>().CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            var content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task RegisterAsync_ValidData_ReturnsOk()
        {
            // Arrange
            var registerModel = new UserGenerator().GenerateRegisterModel();

            var content = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            File.AppendAllText(@"g:\study\text.txt", responseBody + Environment.NewLine);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task UserInfo_ValidToken_ReturnsOkWithUserInfo()
        {
            // Arrange
            _client.SetFakeBearerToken((object)_token);
            // Assert
            var response = await _client.GetAsync("/api/auth/info");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().NotBeNullOrEmpty();

            var userInfo = JsonSerializer.Deserialize<UserDTO>(responseBody);
            userInfo.Should().NotBeNull();
        }

    }

}
