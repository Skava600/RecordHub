using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Domain.Models;

namespace RecordHub.BasketService.Applicatation.Services
{
    public interface IBasketService
    {
        Task<ShoppingCart?> GetBasketAsync(string userId);
        Task<bool> ClearBasketAsync(string userName);
        Task UpdateCartItemAsync(string userName, ShoppingCartItemModel item);
        Task RemoveCartItemAsync(string userName, string productId);
    }
}
