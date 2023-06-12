namespace RecordHub.IdentityService.Domain.Constants
{
    public static class Constants
    {
        public const int TokenLifeTime = 120;
        public static readonly string IdentityDbConnectionString =
                    $"Server={Environment.GetEnvironmentVariable("DB_HOST")};" +
                    $"Port={Environment.GetEnvironmentVariable("DB_PORT")};" +
                    $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                    $"User ID={Environment.GetEnvironmentVariable("DB_USER")};" +
                    $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")}";
    }
}
