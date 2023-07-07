using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;

namespace RecordHub.BasketService.Applicatation.Services
{
    public interface IBasketService
    {
        Task<ShoppingCart?> GetBasketAsync(string? userId, CancellationToken cancellationToken = default);
        Task<bool> ClearBasketAsync(string? userName, CancellationToken cancellationToken = default);
        Task UpdateCartItemAsync(string? userName, ShoppingCartItemModel item, CancellationToken cancellationToken = default);
        Task RemoveCartItemAsync(string? userName, string productId, CancellationToken cancellationToken = default);
        Task CheckoutAsync(BasketCheckoutModel model, CancellationToken cancellationToken = default);
    }
}
