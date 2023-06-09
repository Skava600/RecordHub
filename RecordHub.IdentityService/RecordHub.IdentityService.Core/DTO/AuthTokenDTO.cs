namespace RecordHub.IdentityService.Core.DTO
{
    public class AuthTokenDTO
    {
        public string AccessToken { get; set; }
        public string Email { get; set; }
        public int ExpiresIn { get; set; }
    }
}
