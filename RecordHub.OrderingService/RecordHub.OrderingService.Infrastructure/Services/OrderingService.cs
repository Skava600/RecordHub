using AutoMapper;
using RecordHub.OrderingService.Application.Services;
using RecordHub.OrderingService.Domain.Entities;
using RecordHub.OrderingService.Infrastructure.Data.Repositories;
using RecordHub.Shared.Enums;
using RecordHub.Shared.Exceptions;
using RecordHub.Shared.MassTransit.Models.Order;
using System.Security.Claims;

namespace RecordHub.OrderingService.Infrastructure.Services
{
    public class OrderingService : IOrderingService
    {
        private readonly IMapper _mapper;
        private readonly IOrderingRepository _repository;
        public OrderingService(IMapper mapper, IOrderingRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task AddOrderAsync(BasketCheckoutMessage message, CancellationToken cancellationToken = default)
        {
            var order = _mapper.Map<Order>(message);
            order.State = StatesEnum.Submitted;

            await _repository.AddAsync(order, cancellationToken);
        }

        public async Task ChangeOrderStateAsync(Guid orderId, StatesEnum state, CancellationToken cancellationToken = default)
        {
            var order = await _repository.GetAsync(orderId, cancellationToken);
            if (order == null)
            {
                throw new EntityNotFoundException(nameof(orderId));
            }

            await _repository.UpdateStateAsync(orderId, state, cancellationToken);
        }

        public Task<IEnumerable<Order>> GetUsersOrdersAsync(string userId, ClaimsPrincipal user, CancellationToken cancellationToken = default)
        {
            if (!user.IsInRole("admin") && !userId.Equals(user.FindFirstValue(ClaimTypes.NameIdentifier)))
            {
                throw new UnauthorizedAccessException(nameof(userId));
            }

            return _repository.GetUsersOrdersAsync(userId, cancellationToken);
        }
    }
}
