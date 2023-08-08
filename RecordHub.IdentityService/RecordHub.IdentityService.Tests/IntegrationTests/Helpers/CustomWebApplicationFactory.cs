using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Persistence;
using Testcontainers.PostgreSql;
using WebMotions.Fake.Authentication.JwtBearer;

namespace RecordHub.IdentityService.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private readonly PostgreSqlContainer _sqlContainer;

        public readonly Guid UserId = TestData.User.Id;

        public CustomWebApplicationFactory()
        {
            _sqlContainer = new PostgreSqlBuilder()
               .WithDatabase("AccountTests")
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
                services.RemoveDbContext<AccountDbContext>();

                // Add DB context pointing to test container
                services.AddDbContext<AccountDbContext>(options => { options.UseNpgsql(_sqlContainer.GetConnectionString()); });

                // Ensure schema gets created
                services.EnsureDbCreated<AccountDbContext>();

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
        }

        public new async Task DisposeAsync()
        {
            await _sqlContainer.DisposeAsync();
        }

        private void SeedTestData(IServiceCollection services)
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            userManager.CreateAsync(TestData.User, "123456aA.").Wait();
            userManager.AddToRoleAsync(TestData.User, nameof(Roles.Admin)).Wait();

            TestData.AddressToUpdate.UserId = TestData.User.Id;
            TestData.AddressToDelete.UserId = TestData.User.Id;
            dbContext.Addresses.Add(TestData.AddressToUpdate);
            dbContext.Addresses.Add(TestData.AddressToDelete);
            dbContext.SaveChanges();
        }
    }
}
