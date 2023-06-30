using MassTransit;
using RecordHub.OrderingService.Application.Services;
using RecordHub.OrderingService.Domain.Models;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.OrderingService.Infrastructure.Services
{
    public class OrderingService : IOrderingService
    {
        private readonly IRequestClient<BasketInfoRequest> _client;
        public OrderingService(IRequestClient<BasketInfoRequest> client)
        {
            _client = client;
        }

        public async Task AddOrderAsync(string? userId, OrderModel model, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetResponse<BasketInfoRequest>(new { userId }, cancellationToken);

        }
    }
}
