using RecordHub.OrderingService.Domain.Entities;

namespace RecordHub.OrderingService.Infrastructure.Data.Repositories
{
    public interface IOrderingRepository
    {
        Task AddAsync(
            Order order,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
           Order order,
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
