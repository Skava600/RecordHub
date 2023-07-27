namespace RecordHub.IdentityService.Domain.Data.Entities
{
    public class Address : IBaseEntity
    {
        public Guid Id { get; set; }
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
