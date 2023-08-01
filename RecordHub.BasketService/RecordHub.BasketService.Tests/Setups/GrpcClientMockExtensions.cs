using RecordHub.BasketService.Application.Protos;
using RecordHub.BasketService.Application.Services;

namespace RecordHub.BasketService.Tests.Setups
{
    internal static class GrpcClientMockExtensions
    {
        public static void SetupCheckProductExistenceAsync(this Mock<ICatalogGrpcClient> mock, string productId, ProductReply reply)
        {
            mock.Setup(m => m.CheckProductExistenceAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(reply);
        }
    }
}
