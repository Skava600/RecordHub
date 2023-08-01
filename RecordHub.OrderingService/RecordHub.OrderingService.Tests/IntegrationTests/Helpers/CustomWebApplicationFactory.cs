using Bogus;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.OrderingService.Infrastructure.Consumers;
using RecordHub.OrderingService.Infrastructure.Data;
using RecordHub.OrderingService.Tests.Generators;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace RecordHub.OrderingService.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
               : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        public string UserId = Guid.NewGuid().ToString();
        private readonly PostgreSqlContainer _postgresContainer;

        public CustomWebApplicationFactory()
        {
            _postgresContainer = new PostgreSqlBuilder()
              .WithDatabase("OrderingTests")
              .WithImage("postgres:latest")
              .WithUsername("postgres")
              .WithPassword("postgres")
              .WithCleanUp(true)
              .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<OrderingDbContext>();

                services.AddDbContext<OrderingDbContext>(options => { options.UseNpgsql(_postgresContainer.GetConnectionString()); });

                services.AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<BasketCheckoutConsumer>();
                });

                services.EnsureDbCreated<OrderingDbContext>();

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
            await _postgresContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _postgresContainer.DisposeAsync();
        }

        private void SeedTestData(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();

            var testGeneratorOrders = new OrderGenerator().RuleFor(c => c.UserId, f => UserId);
            var orders = testGeneratorOrders.GenerateBetween(3, 10);

            var orderForUpdating = testGeneratorOrders.Generate();
            orderForUpdating.Id = Guid.Parse("47a3aa1c-21df-46c3-8835-f3ab33dcfcd8");

            dbContext.Orders.AddRange(orders);
            dbContext.Orders.Add(orderForUpdating);
            dbContext.SaveChanges();
        }
    }
}
