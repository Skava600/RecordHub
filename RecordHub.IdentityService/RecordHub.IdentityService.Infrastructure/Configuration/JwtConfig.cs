namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = "RecordHub.IdentityService";
        public string Audience { get; set; } = "Сlient";
        public string Key { get; set; } = "this is my custom Secret key for authentication";
        public int TokenLifeTime { get; set; } = 120;
    }
}
