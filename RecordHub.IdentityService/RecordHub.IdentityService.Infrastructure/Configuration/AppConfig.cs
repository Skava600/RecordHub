namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class AppConfig
    {
        public MassTransitOptions MassTransit { get; set; }
        public JwtConfig Jwt { get; set; }
    }
}
