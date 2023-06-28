namespace RecordHub.IdentityService.Core.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public const string InvalidCredentials = "User was not found or password is incorrect";
        public InvalidCredentialsException(string message) : base(message) { }
        public InvalidCredentialsException() : base(InvalidCredentials) { }
    }
}
