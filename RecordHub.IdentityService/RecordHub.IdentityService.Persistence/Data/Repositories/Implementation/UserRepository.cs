using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;

namespace RecordHub.IdentityService.Persistence.Data.Repositories.Implementation
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AccountDbContext context)
            : base(context)
        {
        }
    }
}
