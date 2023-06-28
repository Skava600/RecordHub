namespace RecordHub.OrderingService.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public Address Address { get; set; }

    }
}
