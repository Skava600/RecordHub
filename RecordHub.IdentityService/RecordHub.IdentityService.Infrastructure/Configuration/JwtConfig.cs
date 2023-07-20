namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class JwtConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int TokenLifeTime { get; set; }
    }
}
