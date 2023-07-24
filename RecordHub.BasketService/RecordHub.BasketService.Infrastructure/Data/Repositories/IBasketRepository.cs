using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Infrastructure.Data.Repositories
{
    public interface IBasketRepository
    {
        Task<Basket?> GetBasketAsync(string userId);
        Task UpdateBasket(Basket basket);
        Task<bool> ClearBasketAsync(string userName);
    }
}
