using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace RecordHub.MailService.Infrastructure.Config
{
    public static class LoggingConfiguration
    {
        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((host, log) =>
             {
                 if (host.HostingEnvironment.IsProduction())
                     log.MinimumLevel.Information();
                 else
                     log.MinimumLevel.Debug();

                 log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
                 log.MinimumLevel.Override("Quartz", LogEventLevel.Information);
                 log.WriteTo.Console();
                 log.ReadFrom.Configuration(builder.Configuration);
             });
        }
    }
}
