﻿namespace RecordHub.IdentityService.Infrastructure.Configuration
{
    public class JwtConfig
    {
        public string Issuer { get; set; } = "RecordHub.IdentityService"; // издатель токена
        public string Audience { get; set; } = "Client"; // потребитель токена
        public string Key { get; set; }   // ключ для шифрации
    }
}
