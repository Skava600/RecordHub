namespace RecordHub.Shared.MassTransit.Models.Order
{
    public class BasketInfoResponse
    {
        public string UserId { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
        public double Price { get; set; }

    }
}
