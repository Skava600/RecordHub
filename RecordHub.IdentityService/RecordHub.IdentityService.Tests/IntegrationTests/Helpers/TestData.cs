using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Tests.Generators;

namespace RecordHub.IdentityService.Tests.IntegrationTests.Helpers
{
    internal static class TestData
    {
        public static User User = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@gmail.com",
            Name = "test",
            Surname = "test",
            SecurityStamp = Guid.NewGuid().ToString("D"),
            EmailConfirmed = true,
            UserName = "test",
            PhoneNumber = "375333274122",
        };

        public static Address AddressToUpdate = new AddressGenerator().Generate();

        public static Address AddressToDelete = new AddressGenerator().Generate();
    }
}
