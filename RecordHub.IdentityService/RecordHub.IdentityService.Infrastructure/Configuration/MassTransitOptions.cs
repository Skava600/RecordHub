namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class MassTransitOptions
    {
        public string Host { get; set; }
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
