namespace RecordHub.Shared.Services.Logging.Settings
{
    public class AppLoggingSettings
    {
        public GeneralSettings General { get; set; }
        public PostgreSqlSettings PostgresSettings { get; set; }

        public class GeneralSettings
        {
            public string RestrictedToMinimumLevel { get; set; }
        }
        public class PostgreSqlSettings
        {
            public string TableName { get; set; }
            public string Schema { get; set; }
            public string ConnectionStringName { get; set; }
        }

    }
}