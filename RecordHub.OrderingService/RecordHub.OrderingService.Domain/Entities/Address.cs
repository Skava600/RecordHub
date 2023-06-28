namespace RecordHub.OrderingService.Domain.Entities
{
    public class Address : BaseEntity
    {
        public Guid UserId { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string? Korpus { get; set; }
        public string? Appartment { get; set; }
        public string Postcode { get; set; }
    }
}
