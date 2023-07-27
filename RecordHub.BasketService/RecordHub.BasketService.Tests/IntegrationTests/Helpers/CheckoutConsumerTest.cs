using MassTransit;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.BasketService.Tests.IntegrationTests.Helpers
{
    public class CheckoutConsumerTest : IConsumer<BasketCheckoutMessage>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutMessage> context)
        {

        }
    }
}
