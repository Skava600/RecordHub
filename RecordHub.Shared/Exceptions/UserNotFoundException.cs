namespace RecordHub.Shared.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public const string UserNotFound = "User was not found";
        public UserNotFoundException(string message) : base(message) { }
        public UserNotFoundException() : base(UserNotFound) { }
    }
}
