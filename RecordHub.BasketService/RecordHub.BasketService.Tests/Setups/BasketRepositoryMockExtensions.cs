using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Infrastructure.Data.Repositories;

namespace RecordHub.BasketService.Tests.Setups
{
    internal static class BasketRepositoryMockExtensions
    {
        public static void SetupClearBasketAsync(this Mock<IBasketRepository> basketRepositoryMock, string userName, bool returnValue)
        {
            basketRepositoryMock
                .Setup(m => m.ClearBasketAsync(userName))
                .ReturnsAsync(returnValue)
                .Verifiable();
        }

        public static void SetupGetBasketAsync(this Mock<IBasketRepository> basketRepositoryMock, string userName, Basket basket)
        {
            basketRepositoryMock
                .Setup(m => m.GetBasketAsync(userName))
                .ReturnsAsync(basket)
                .Verifiable();
        }
    }
}
