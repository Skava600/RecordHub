using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.BasketService.Tests.Generators;
using StackExchange.Redis;
using System.Text.Json;
using Testcontainers.Redis;
using WebMotions.Fake.Authentication.JwtBearer;

namespace RecordHub.BasketService.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
            : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        public string UserId = Guid.NewGuid().ToString();

        private readonly RedisContainer _redisContainer;

        public CustomWebApplicationFactory()
        {
            _redisContainer = new RedisBuilder()
               .WithCleanUp(true)
               .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.ConfigureTestServices(services =>
            {
                services.RemoveApplicationRedis();

                services.AddTestContainersRedis(_redisContainer.GetConnectionString());
                services.AddMockedGrpc();
                services.AddMockedMassTransit();

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                }).AddFakeJwtBearer();

                SeedTestData(services);
            });

            builder.UseEnvironment("Development");
        }

        public async Task InitializeAsync()
        {
            await _redisContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _redisContainer.DisposeAsync();
        }

        private void SeedTestData(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var connectionMultiplexer = serviceProvider.GetRequiredService<IConnectionMultiplexer>();

            var database = connectionMultiplexer.GetDatabase();

            var basketItems = BasketItemGenerator.GenerateBeetween(3, 10);

            database.StringSet(UserId, JsonSerializer.Serialize(basketItems));
        }
    }
}