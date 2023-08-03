using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.MailService.Infrastructure.Consumers;
using RecordHub.MailService.Infrastructure.Settings;
using RecordHub.MailService.Tests.Generators;
using Testcontainers.PostgreSql;

namespace RecordHub.MailService.Tests.IntegrationTests.Helpers
{
    public class CustomWebApplicationFactory<TProgram>
                  : WebApplicationFactory<TProgram>, IAsyncLifetime where TProgram : class
    {
        private readonly PostgreSqlContainer _sqlContainer;
        private readonly MailSettings _settings;

        public CustomWebApplicationFactory()
        {
            _settings = new MailSettingsGenerator().Generate();
            _sqlContainer = new PostgreSqlBuilder()
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

                services.Configure<MailSettings>(opts =>
                {
                    opts.Host = _settings.Host;
                    opts.Port = _settings.Port;
                    opts.UseSSL = _settings.UseSSL;
                    opts.UserName = _settings.UserName;
                    opts.From = _settings.From;
                    opts.UseStartTls = _settings.UseStartTls;
                    opts.Password = _settings.Password;
                    opts.DisplayName = _settings.DisplayName;
                });

                services.AddHangfire(
                    x => x.UsePostgreSqlStorage(_sqlContainer.GetConnectionString()));

                services.AddMassTransitTestHarness(x =>
                {
                    x.AddConsumer<SendEmailConsumer>();
                });
            });

            builder.UseEnvironment("Development");
        }

        public Task InitializeAsync()
        {
            return _sqlContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            await _sqlContainer.DisposeAsync();
        }
    }
}
