using RecordHub.OrderingService.Domain.Models;

namespace RecordHub.OrderingService.Application.Services
{
    public interface IOrderingService
    {
        Task AddOrderAsync(string? userId, OrderModel model, CancellationToken cancellationToken = default);
    }
}
