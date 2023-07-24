using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;

namespace RecordHub.IdentityService.Tests.Setups
{
    internal static class IAddressRepositoryMockExtensions
    {
        public static void SetupGetAddressesByUserId(this Mock<IAddressRepository> addressRepositoryMock, IEnumerable<Address> addresses)
        {
            addressRepositoryMock
                .Setup(m => m.GetAddressesByUserId(It.IsAny<Guid>()))
                .Returns(addresses);
        }

        public static void SetupGetByIdAsync(this Mock<IAddressRepository> addressRepositoryMock, Address? address, CancellationToken cancellationToken)
        {
            addressRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(address);
        }
    }
}
