using Microsoft.EntityFrameworkCore;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;

namespace RecordHub.IdentityService.Persistence.Data.Repositories.Implementation
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        private readonly DbSet<Address> addresses;
        public AddressRepository(AccountDbContext context) : base(context)
        {
            addresses = context.Addresses;
        }

        public IEnumerable<Address> GetAddressesByUserId(Guid userId)
        {
            return addresses.Where(a => a.UserId == userId);
        }
    }
}
