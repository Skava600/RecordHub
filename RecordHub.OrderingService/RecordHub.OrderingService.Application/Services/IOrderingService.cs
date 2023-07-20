using RecordHub.OrderingService.Domain.Entities;
using RecordHub.Shared.Enums;
using RecordHub.Shared.MassTransit.Models.Order;
using System.Security.Claims;

namespace RecordHub.OrderingService.Application.Services
{
    public interface IOrderingService
    {
        Task AddOrderAsync(
            BasketCheckoutMessage message,
            CancellationToken cancellationToken = default);

        Task ChangeOrderStateAsync(
            Guid orderId,
            StatesEnum state,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Order>> GetUsersOrdersAsync(
            string userId,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default);
    }
}
