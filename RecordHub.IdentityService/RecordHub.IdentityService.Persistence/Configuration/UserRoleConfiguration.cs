using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecordHub.IdentityService.Domain.Enum;
using System.Data;

namespace RecordHub.IdentityService.Persistence.Configuration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            builder.HasData(Enum.GetNames<Roles>().Select(role => new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = role,
                NormalizedName = role.ToUpper()
            }));
        }
    }
}
