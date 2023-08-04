using RecordHub.CatalogService.Application.Services;

namespace RecordHub.CatalogService.Tests.Setups
{
    public static class CacheServiceMockExtensions
    {
        public static Mock<IRedisCacheService> SetupCacheServiceForGetAsync<T>(this Mock<IRedisCacheService> cacheServiceMock, T? value)
        {
            cacheServiceMock
                .Setup(cache => cache.GetAsync<T>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value);
            return cacheServiceMock;
        }

        public static Mock<IRedisCacheService> SetupCacheServiceForGetAsync<T>(this Mock<IRedisCacheService> cacheServiceMock, IEnumerable<T>? value)
        {
            cacheServiceMock
                .Setup(cache => cache.GetAsync<IEnumerable<T>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(value);
            return cacheServiceMock;
        }
    }
}
