namespace RecordHub.IdentityService.Domain.Models
{
    public class AddressModel
    {
        public string State { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string? Korpus { get; set; }
        public string? Appartment { get; set; }
        public string Postcode { get; set; }
    }
}
