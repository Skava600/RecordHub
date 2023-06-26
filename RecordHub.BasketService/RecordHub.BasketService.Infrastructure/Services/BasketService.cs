using RecordHub.BasketService.Applicatation.Exceptions;
using RecordHub.BasketService.Applicatation.Services;
using RecordHub.BasketService.Domain.Entities;
using RecordHub.BasketService.Infrastructure.Data.Repositories;

namespace RecordHub.BasketService.Infrastructure.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _repo;

        public BasketService(IBasketRepository repo)
        {
            _repo = repo;
        }

        public Task<bool> ClearBasketAsync(string userName)
        {
            return _repo.ClearBasketAsync(userName);
        }


        public Task<ShoppingCart?> GetBasketAsync(string userId)
        {
            return _repo.GetBasketAsync(userId);
        }

        public async Task RemoveCartItemAsync(string userName, string productId)
        {
            var basket = await _repo.GetBasketAsync(userName);

            if (basket != null)
            {
                throw new BasketIsEmptyException();
            }

            var itemToDelete = basket.Items.Find(i => i.ProductId.Equals(productId));
            if (itemToDelete == null)
            {
                throw new ItemMissingInBasketException();
            }

            basket.Items.Remove(itemToDelete);

            await _repo.UpdateBasket(basket);
        }

        public async Task UpdateCartItemAsync(string userName, ShoppingCartItem item)
        {
            var basket = await _repo.GetBasketAsync(userName);

            if (basket == null)
            {
                basket = new ShoppingCart
                {
                    UserName = userName,
                    Items = new List<ShoppingCartItem> { item }
                };
            }

            var oldItem = basket.Items.Find(i => i.ProductId.Equals(item.ProductId));
            if (oldItem == null)
            {
                basket.Items.Add(item);
            }
            else
            {
                oldItem = item;
            }

            await _repo.UpdateBasket(basket);
        }
    }
}
