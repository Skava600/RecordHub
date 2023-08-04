namespace RecordHub.CatalogService.Infrastructure.Config
{
    public class RedisConfig
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }
        public int AbsoluteExpirationMinutes { get; set; }
        public int SlidingExpirationMinutes { get; set; }
    }
}
