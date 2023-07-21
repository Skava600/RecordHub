using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Tests.Controllers
{
    public static class AddressControllerSetup
    {
        public static void SetupAddAsync(this Mock<IAddressService> addressServiceMock)
        {
            addressServiceMock
                .Setup(m => m.AddAsync(It.IsAny<string>(), It.IsAny<AddressModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupUpdateAsync(this Mock<IAddressService> addressServiceMock)
        {
            addressServiceMock
                .Setup(m => m.UpdateAsync(It.IsAny<Guid>(), It.IsAny<AddressModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupDeleteAsync(this Mock<IAddressService> addressServiceMock)
        {
            addressServiceMock
                .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }
    }
}
