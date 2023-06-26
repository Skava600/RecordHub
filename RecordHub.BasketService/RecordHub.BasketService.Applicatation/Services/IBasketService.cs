using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Applicatation.Services
{
    public interface IBasketService
    {
        Task<ShoppingCart?> GetBasketAsync(string userId);
        Task<bool> ClearBasketAsync(string userName);
        Task UpdateCartItemAsync(string userName, ShoppingCartItem item);
        Task RemoveCartItemAsync(string userName, string productId);
    }
}
