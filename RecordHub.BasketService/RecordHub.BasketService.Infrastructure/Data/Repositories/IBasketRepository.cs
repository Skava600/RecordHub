using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Infrastructure.Data.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart?> GetBasketAsync(string userId);
        Task UpdateBasket(ShoppingCart basket);
        Task<bool> ClearBasketAsync(string userName);
    }
}
