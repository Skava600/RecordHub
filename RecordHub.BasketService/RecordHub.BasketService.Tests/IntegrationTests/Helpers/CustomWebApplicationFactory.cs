using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.BasketService.Domain.Entities;
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

                services.RemoveProductionRedis();

                services.AddTestContainersRedis(_redisContainer.GetConnectionString());

                services.AddMockedGrpc();

                var publishEndpoint = new Mock<IPublishEndpoint>();
                services.AddScoped<IPublishEndpoint>(e => publishEndpoint.Object);

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

            // Get a reference to the database
            var database = connectionMultiplexer.GetDatabase();


            var basket = new Basket(UserId);
            var basketItems = BasketItemGenerator.GenerateBeetween(3, 10);
            foreach (var item in basketItems)
            {
                basket.UpdateItem(item);
            }

            database.StringSet(UserId, JsonSerializer.Serialize(basket.Items));
        }
    }
}