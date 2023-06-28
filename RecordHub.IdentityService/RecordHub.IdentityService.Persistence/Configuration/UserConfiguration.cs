using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany<Address>(x => x.Addresses)
                .WithOne()
                .HasForeignKey(x => x.UserId);
        }
    }
}
