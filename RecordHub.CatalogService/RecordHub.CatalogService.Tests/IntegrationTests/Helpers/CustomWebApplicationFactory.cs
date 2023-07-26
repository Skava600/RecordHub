using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.CatalogService.Infrastructure;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace RecordHub.CatalogService.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
     : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private readonly PostgreSqlContainer _container;

        public CustomWebApplicationFactory()
        {
            _container = new PostgreSqlBuilder()
               .WithDatabase("CatalogTests")
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
                services.RemoveDbContext<ApplicationDbContext>();

                // Add DB context pointing to test container
                services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(_container.GetConnectionString()); });

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
            await _container.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _container.DisposeAsync();
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
