using RecordHub.BasketService.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace RecordHub.BasketService.Infrastructure.Data.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IConnectionMultiplexer _redisCache;
        private readonly IDatabase database;
        public BasketRepository(IConnectionMultiplexer redisCache)
        {
            _redisCache = redisCache;
            database = redisCache.GetDatabase();
        }

        public async Task<bool> ClearBasketAsync(string userName)
        {
            var result = await database.StringGetDeleteAsync(userName);
            if (result == RedisValue.Null)
            {
                return false;
            }

            return true;
        }

        public async Task<ShoppingCart?> GetBasketAsync(string userId)
        {
            var basket = await database.StringGetAsync(userId);
            if (String.IsNullOrEmpty(basket))
                return null;

            return JsonSerializer.Deserialize<ShoppingCart>(basket);
        }

        public Task UpdateBasket(ShoppingCart basket)
        {
            return database.StringSetAsync(basket.UserName, JsonSerializer.Serialize(basket));
        }
    }


}
