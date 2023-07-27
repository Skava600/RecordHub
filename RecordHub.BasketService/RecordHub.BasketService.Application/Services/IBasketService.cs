using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;

namespace RecordHub.BasketService.Application.Services
{
    public interface IBasketService
    {
        Task<Basket?> GetBasketAsync(
            string? userId,
            CancellationToken cancellationToken = default);

        Task<bool> ClearBasketAsync(
            string? userName,
            CancellationToken cancellationToken = default);

        Task UpdateBasketItemAsync(
            string? userName,
            BasketItemModel item,
            CancellationToken cancellationToken = default);

        Task RemoveBasketItemAsync(
            string? userName,
            string productId,
            CancellationToken cancellationToken = default);

        Task CheckoutAsync(
            BasketCheckoutModel model,
            CancellationToken cancellationToken = default);
    }
}
