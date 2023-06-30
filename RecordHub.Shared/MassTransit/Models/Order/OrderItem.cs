namespace RecordHub.Shared.MassTransit.Models.Order
{
    public class OrderItem
    {
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
