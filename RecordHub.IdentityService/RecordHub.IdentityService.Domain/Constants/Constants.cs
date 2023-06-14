namespace RecordHub.IdentityService.Domain.Constants
{
    public static class Constants
    {
        public const int TokenLifeTime = 120;
        public static readonly string IdentityDbConnectionString =
                    $"Server={Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost"};" +
                    $"Port={Environment.GetEnvironmentVariable("DB_PORT") ?? "5432"};" +
                    $"Database={Environment.GetEnvironmentVariable("DB_NAME") ?? "RecordHub.IdentityService"};" +
                    $"User ID={Environment.GetEnvironmentVariable("DB_USER") ?? "postgres"};" +
                    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "123456"}";
    }
}
