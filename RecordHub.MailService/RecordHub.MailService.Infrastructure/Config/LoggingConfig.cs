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
            LogEventLevel logLevel;
            if (builder.Environment.IsProduction())
            {
                logLevel = LogEventLevel.Information;
            }
            else
            {
                logLevel = LogEventLevel.Debug;
            }

            var log = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .MinimumLevel.Is(logLevel)
                .WriteTo.Console(restrictedToMinimumLevel: logLevel)
                .ReadFrom.Configuration(builder.Configuration);

            builder.Logging.AddSerilog(log.CreateLogger(), false);
        }
    }
}
