namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class JwtConfig
    {
        public string Issuer = "RecordHub.IdentityService"; // издатель токена
        public string Audience = "Client"; // потребитель токена
        public string Key = "mysupersecret_secretkey!123";   // ключ для шифрации
    }
}
