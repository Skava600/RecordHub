using RecordHub.Shared.Enums;

namespace RecordHub.OrderingService.Domain.Entities
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }
        public double TotalPrice { get; set; }
        public string Address { get; set; }
        public StatesEnum State { get; set; }
    }
}
