using RecordHub.BasketService.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace RecordHub.BasketService.Infrastructure.Data.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase database;

        public BasketRepository(IConnectionMultiplexer redisCache)
        {
            database = redisCache.GetDatabase();
        }

        public async Task<bool> ClearBasketAsync(string userName)
        {
            var result = await database.StringGetDeleteAsync(userName);

            return result != RedisValue.Null;
        }

        public async Task<Basket?> GetBasketAsync(string userId)
        {
            var basket = await database.StringGetAsync(userId);
            if (String.IsNullOrEmpty(basket))
            {
                return null;
            }

            return JsonSerializer.Deserialize<Basket>(basket);
        }

        public Task UpdateBasket(Basket basket)
        {
            return database.StringSetAsync(basket.UserName, JsonSerializer.Serialize(basket));
        }
    }
}
