using Microsoft.AspNetCore.Identity;

namespace RecordHub.IdentityService.Domain.Data.Entities
{
    public class User : IdentityUser<Guid>, IBaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}
