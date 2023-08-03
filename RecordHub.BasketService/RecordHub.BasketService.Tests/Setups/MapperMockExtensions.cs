using AutoMapper;

namespace RecordHub.BasketService.Tests.Setups
{
    internal static class MapperMockExtensions
    {
        public static void SetupMap<TSource, TDestination>(this Mock<IMapper> mapperMock, TSource source, TDestination destination)
        {
            mapperMock
                .Setup(m => m.Map<TDestination>(source))
                .Returns(destination)
                .Verifiable();
        }

        public static void SetupMap<TSource, TDestination>(this Mock<IMapper> mapperMock, IEnumerable<TSource> source, IEnumerable<TDestination> destination)
        {
            mapperMock
                .Setup(m => m.Map<IEnumerable<TDestination>>(It.IsAny<IEnumerable<TSource>>()))
                .Returns(destination)
                .Verifiable();
        }
    }
}
