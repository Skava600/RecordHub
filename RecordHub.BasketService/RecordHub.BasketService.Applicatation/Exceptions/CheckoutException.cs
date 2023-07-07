namespace RecordHub.BasketService.Applicatation.Exceptions
{
    public class CheckoutException : Exception
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
