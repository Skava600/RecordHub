namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = "identity";
        public string Audience { get; set; } = "client";
        public string Key { get; set; } = "SecretKey";
        public int TokenLifeTime { get; set; }
    }
}
