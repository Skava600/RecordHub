namespace RecordHub.BasketService.Application.Exceptions
{
    public class BasketIsEmptyException : Exception
    {
        private static readonly string Message = "This operation can't be applied to empty basket.";

        public BasketIsEmptyException()
            : base(Message)
        {
        }

        public BasketIsEmptyException(string message)
            : base(message)
        {
        }
    }
}
