using AutoMapper;

namespace RecordHub.OrderingService.Tests.Setups
{
    internal static class MapperMockExtension
    {
        public static void SetupMap<TSource, TDestination>(this Mock<IMapper> mapperMock, TSource source, TDestination destination)
        {
            mapperMock
                .Setup(m => m.Map<TDestination>(source))
                .Returns(destination)
                .Verifiable();
        }
    }
}
