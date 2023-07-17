namespace RecordHub.BasketService.Application.Exceptions
{
    public class CheckoutException : Exception
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
