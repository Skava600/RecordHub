using Newtonsoft.Json;
using RecordHub.BasketService.Domain.Entities;
using StackExchange.Redis;

namespace RecordHub.BasketService.Infrastructure.Data.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IConnectionMultiplexer _redisCache;
        private readonly IDatabaseAsync _redisDb;
        public BasketRepository(IConnectionMultiplexer redisCache)
        {
            _redisCache = redisCache;
            _redisDb = redisCache.GetDatabase();
        }

        public async Task<bool> ClearBasketAsync(string userName)
        {
            var result = await _redisDb.StringGetDeleteAsync(userName);
            if (result == RedisValue.Null)
            {
                return false;
            }

            return true;
        }

        public async Task<ShoppingCart?> GetBasketAsync(string userId)
        {
            var basket = await _redisDb.StringGetAsync(userId);
            if (String.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public Task UpdateBasket(ShoppingCart basket)
        {
            return _redisDb.StringSetAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        }
    }


}
