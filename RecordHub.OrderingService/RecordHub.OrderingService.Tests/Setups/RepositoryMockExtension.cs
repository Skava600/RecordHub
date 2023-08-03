using RecordHub.OrderingService.Domain.Entities;
using RecordHub.OrderingService.Infrastructure.Data.Repositories;

namespace RecordHub.OrderingService.Tests.Setups
{
    internal static class RepositoryMockExtension
    {
        public static void SetupRepositoryMockAddAsync(this Mock<IOrderingRepository> repositoryMock, CancellationToken cancellationToken)
        {
            repositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Order>(), cancellationToken))
                .Returns(Task.CompletedTask);
        }

        public static void SetupRepositoryMockGetAsync(this Mock<IOrderingRepository> repositoryMock, Order order, CancellationToken cancellationToken)
        {
            repositoryMock.Setup(repo => repo.GetAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(order);
        }

        public static void SetupRepositoryMockGetUsersOrdersAsync(this Mock<IOrderingRepository> repositoryMock, IEnumerable<Order> orders, CancellationToken cancellationToken)
        {
            repositoryMock.Setup(repo => repo.GetUsersOrdersAsync(It.IsAny<string>(), cancellationToken))
                .ReturnsAsync(orders);
        }
    }
}
