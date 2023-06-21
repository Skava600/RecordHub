using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecordHub.Shared.Services.Logging.Settings;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Events;

namespace RecordHub.MailService.Infrastructure.Config
{
    public static class LoggingConfiguration
    {
        internal static readonly string OutputTemplate =
            @"[{Timestamp:yy-MM-dd HH:mm:ss} {Level}]{ApplicationName}:{SourceContext}{NewLine}Message:{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";


        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            var config = builder.Configuration;
            var settings = config.GetSection(nameof(AppLoggingSettings)).Get<AppLoggingSettings>();
            string restrictedToMinimumLevel = settings.General.RestrictedToMinimumLevel;
            if (!Enum.TryParse<LogEventLevel>(restrictedToMinimumLevel, out var logLevel))
            {
                logLevel = LogEventLevel.Debug;
            }

            var log = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .Enrich.FromLogContext()
                .Enrich.With(new PropertyEnricher("ApplicationName", config.GetValue<string>("ApplicationName")))
                .Enrich.WithMachineName()
                .WriteTo.Console(restrictedToMinimumLevel: logLevel);

            builder.Logging.AddSerilog(log.CreateLogger(), false);
        }
    }
}
