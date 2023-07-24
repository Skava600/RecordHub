using AutoMapper;

namespace RecordHub.IdentityService.Tests.Setups
{
    public static class MapperMockExtensions
    {
        public static void SetupMap<TSource, TDestination>(this Mock<IMapper> mapperMock, TSource source, TDestination destination)
        {
            mapperMock
                .Setup(m => m.Map<TDestination>(source))
                .Returns(destination);
        }
    }
}
