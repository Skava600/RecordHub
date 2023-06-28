using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Persistence.Data.Repositories.Generic
{
    public interface IAddressRepository : IRepository<Address>
    {
        IEnumerable<Address> GetAddressesByUserId(Guid userId);
    }
}
