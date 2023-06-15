namespace RecordHub.CatalogService.Domain.Constants
{
    public static class CatalogConstants
    {
        public static readonly string DbConnectionString =
            $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
            $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
            $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
            $"User ID={Environment.GetEnvironmentVariable("DB_USER")};" +
            $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
    }
}
