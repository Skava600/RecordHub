using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Infrastructure.Config;
using System.Text.Json;

namespace RecordHub.CatalogService.Infrastructure.Services
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly DistributedCacheEntryOptions cacheOptions;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public RedisCacheService(IDistributedCache cache, IOptions<RedisConfig> redisOption)
        {
            _cache = cache;
            _jsonSerializerOptions = new JsonSerializerOptions();
            cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(redisOption.Value.SlidingExpirationMinutes))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(redisOption.Value.AbsoluteExpirationMinutes));
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(key, cancellationToken);
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var cached = await _cache.GetStringAsync(key, cancellationToken);

            return string.IsNullOrEmpty(cached)
                ? default
                : JsonSerializer.Deserialize<T>(cached, _jsonSerializerOptions);
        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            var serialized = JsonSerializer.Serialize(value, _jsonSerializerOptions);
            await _cache.SetStringAsync(key, serialized, cacheOptions, cancellationToken);
        }
    }
}
