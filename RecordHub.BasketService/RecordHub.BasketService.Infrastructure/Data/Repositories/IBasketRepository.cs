using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Infrastructure.Data.Repositories
{
    public interface IBasketRepository
    {
        Task<IEnumerable<BasketItem>?> GetBasketAsync(string userId);
        Task UpdateBasket(string userName, IEnumerable<BasketItem> items);
        Task<bool> ClearBasketAsync(string userName);
    }
}
