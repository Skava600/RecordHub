using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Core.DTO
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
    }
}
