namespace RecordHub.BasketService.Application.Exceptions
{
    public class ItemMissingInBasketException : Exception
    {
        public static readonly string Message = "Item is missing to operate.";

        public ItemMissingInBasketException()
            : base(Message)
        {
        }
    }
}
