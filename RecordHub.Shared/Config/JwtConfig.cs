namespace RecordHub.Shared.Config
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = "RecordHub.IdentityService"; // издатель токена
        public string Audience { get; set; } = "Client"; // потребитель токена
        public string Key { get; set; } = "mysecretkey123";  // ключ для шифрации
    }
}
