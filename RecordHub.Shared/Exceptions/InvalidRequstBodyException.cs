namespace RecordHub.Shared.Exceptions
{
    public class InvalidRequestBodyException : Exception
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
