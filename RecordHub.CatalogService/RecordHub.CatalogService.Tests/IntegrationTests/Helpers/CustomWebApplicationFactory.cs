using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.CatalogService.Infrastructure;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using WebMotions.Fake.Authentication.JwtBearer;

namespace RecordHub.CatalogService.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private readonly PostgreSqlContainer _sqlContainer;
        private readonly RedisContainer _redisContainer;

        public CustomWebApplicationFactory()
        {
            _sqlContainer = new PostgreSqlBuilder()
               .WithDatabase("CatalogTests")
               .WithImage("postgres:latest")
               .WithUsername("postgres")
               .WithPassword("postgres")
               .WithCleanUp(true)
               .Build();

            _redisContainer = new RedisBuilder()
                .WithCleanUp(true)
                .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<ApplicationDbContext>();

                // Add DB context pointing to test container
                services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(_sqlContainer.GetConnectionString()); });
                services.AddRedisTestCache(_redisContainer.GetConnectionString());

                // Ensure schema gets created
                services.EnsureDbCreated<ApplicationDbContext>();

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
            await _sqlContainer.StartAsync();
            await _redisContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _sqlContainer.DisposeAsync();
            await _redisContainer.DisposeAsync();
        }

        private void SeedTestData(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            dbContext.Artists.AddRange(TestData.Artists);
            dbContext.Labels.AddRange(TestData.Labels);
            dbContext.Countries.AddRange(TestData.Countries);
            dbContext.Styles.AddRange(TestData.Styles);
            dbContext.Records.AddRange(TestData.Records);
            dbContext.SaveChanges();
        }
    }
}
