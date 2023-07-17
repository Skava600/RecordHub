using MassTransit;
using Microsoft.Extensions.Logging;
using RecordHub.OrderingService.Application.Services;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.OrderingService.Infrastructure.Consumers
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutMessage>
    {
        private readonly IOrderingService _orderingService;
        private readonly ILogger<BasketCheckoutConsumer> _logger;

        public BasketCheckoutConsumer(IOrderingService orderingService, ILogger<BasketCheckoutConsumer> logger)
        {
            this._orderingService = orderingService;
            this._logger = logger;
        }

        public async Task Consume(ConsumeContext<BasketCheckoutMessage> context)
        {
            await _orderingService.AddOrderAsync(context.Message);
            _logger.LogInformation("BasketCheckoutEvent consumed successfully. Creator user id : {userId}", context.Message.UserId);
        }
    }
}
