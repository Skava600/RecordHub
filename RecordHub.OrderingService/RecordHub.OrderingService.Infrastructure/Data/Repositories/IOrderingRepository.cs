using RecordHub.OrderingService.Domain.Entities;
using RecordHub.Shared.Enums;

namespace RecordHub.OrderingService.Infrastructure.Data.Repositories
{
    public interface IOrderingRepository
    {
        Task AddAsync(
            Order order,
            CancellationToken cancellationToken = default);

        Task UpdateStateAsync(
            Guid orderId,
            StatesEnum state,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Order?> GetAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Order>> GetUsersOrdersAsync(
            string userId,
            CancellationToken cancellationToken = default);
    }
}
