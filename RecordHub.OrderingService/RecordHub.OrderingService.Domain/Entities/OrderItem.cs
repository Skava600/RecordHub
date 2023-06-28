namespace RecordHub.OrderingService.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid RecordId { get; set; }
        public Guid OrderId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public string Titile { get; set; }
    }
}
