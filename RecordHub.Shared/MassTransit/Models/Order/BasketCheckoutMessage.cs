namespace RecordHub.Shared.MassTransit.Models.Order
{
    public class BasketCheckoutMessage : IntegrationBaseEvent
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string UserId { get; set; }
        public double TotalPrice { get; set; }
        public string Address { get; set; }
        public IEnumerable<OrderItemModel> Items { get; set; }
    }
}
