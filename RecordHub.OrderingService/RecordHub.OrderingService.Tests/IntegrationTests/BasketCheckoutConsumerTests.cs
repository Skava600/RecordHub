using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordHub.OrderingService.Infrastructure.Data;
using RecordHub.OrderingService.Tests.Generators;
using RecordHub.OrderingService.Tests.IntegrationTests.Helpers;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.OrderingService.Tests.IntegrationTests
{
    public class BasketCheckoutConsumerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public BasketCheckoutConsumerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task BasketCheckoutConsumer_ConsumesBasketCheckoutMessage_AddsOrderToDatabase()
        {
            // Arrange

            var scope = _factory.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var harness = scope.ServiceProvider.GetRequiredService<ITestHarness>();
            var context = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
            await harness.Start();

            // Create a fake BasketCheckoutMessage
            var basketCheckoutMessage = new BasketCheckoutMessageGenerator().Generate();

            // Act
            await harness.Bus.Publish(basketCheckoutMessage);
            await Task.Delay(TimeSpan.FromSeconds(2));

            (await harness.Consumed.Any<BasketCheckoutMessage>()).Should().BeTrue();
            (await harness.Published.Any<BasketCheckoutMessage>()).Should().BeTrue();

            var createdOrder = context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.UserId == basketCheckoutMessage.UserId);

            createdOrder
                .Should()
                .NotBeNull();

            createdOrder.UserId
                .Should()
                .Be(basketCheckoutMessage.UserId);

            createdOrder.TotalPrice
                .Should()
                .Be(basketCheckoutMessage.TotalPrice);

            createdOrder.Items
                .Should()
                .HaveCount(basketCheckoutMessage.Items.Count());

            await harness.Stop();
        }
    }
}
