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

        private readonly RedisContainer _container;

        public CustomWebApplicationFactory()
        {
            _container = new RedisBuilder()
               .WithCleanUp(true)
               .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.ConfigureTestServices(services =>
            {

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(IConnectionMultiplexer));

                services.Remove(dbConnectionDescriptor);

                services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var multConfig = ConfigurationOptions.Parse(_container.GetConnectionString(), true);
                    multConfig.AbortOnConnectFail = false;

                    return ConnectionMultiplexer.Connect(multConfig);
                });

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
            await _container.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _container.DisposeAsync();
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

            database.StringSet(UserId, JsonSerializer.Serialize(basket));
            basket = JsonSerializer.Deserialize<Basket>(database.StringGet(UserId));
        }
    }
}