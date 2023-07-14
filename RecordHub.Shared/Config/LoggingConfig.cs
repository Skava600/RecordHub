using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NpgsqlTypes;
using RecordHub.Shared.Services.Logging.Settings;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace RecordHub.Shared.Config
{
    public static class LoggingConfiguration
    {
        internal static readonly string OutputTemplate =
            @"[{Timestamp:yy-MM-dd HH:mm:ss} {Level}]{ApplicationName}:{SourceContext}{NewLine}Message:{Message}{NewLine}in method {MemberName} at {FilePath}:{LineNumber}{NewLine}{Exception}{NewLine}";

        static IDictionary<string, ColumnWriterBase> ColumnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
            { "message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text) },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
            { "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
            { "props_test", new PropertiesColumnWriter(NpgsqlDbType.Jsonb) },
            { "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text, "l") },
        };

        public static void ConfigureSerilog(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            var config = builder.Configuration;
            var settings = config.GetSection(nameof(AppLoggingSettings)).Get<AppLoggingSettings>();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
            var tableName = settings.PostgresSettings.TableName;
            var schema = settings.PostgresSettings.Schema;
            LogEventLevel logLevel;
            if (builder.Environment.IsProduction())
            {
                logLevel = LogEventLevel.Information;
            }
            else
            {
                logLevel = LogEventLevel.Debug;
            }

            var sqlOptions = new PostgreSqlOptions
            {
                NeedAutoCreateTable = true,
                SchemaName = schema,
                TableName = tableName,
            };
            if (builder.Environment.IsDevelopment())
            {
                sqlOptions.Period = new TimeSpan(0, 0, 0, 1);
                sqlOptions.BatchSizeLimit = 1;
            }

            var log = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Quartz", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.With(new PropertyEnricher("ApplicationName", config.GetValue<string>("ApplicationName")))
                .Enrich.WithMachineName()
                .WriteTo.Console(restrictedToMinimumLevel: logLevel)
                .WriteTo.PostgreSQL(
                    connectionString: connectionString,
                    needAutoCreateTable: true,
                    schemaName: schema,
                    tableName: tableName,
                    restrictedToMinimumLevel: logLevel,
                    columnOptions: ColumnWriters);
            builder.Logging.AddSerilog(log.CreateLogger(), false);
        }
    }
}
