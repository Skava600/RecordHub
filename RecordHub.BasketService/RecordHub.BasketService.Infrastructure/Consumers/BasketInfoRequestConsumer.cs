using MassTransit;
using RecordHub.BasketService.Infrastructure.Data.Repositories;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.BasketService.Infrastructure.Consumers
{
    public class BasketInfoRequestConsumer : IConsumer<BasketInfoRequest>
    {
        private readonly IBasketRepository _repo;

        public BasketInfoRequestConsumer(IBasketRepository repo)
        {
            _repo = repo;
        }

        public async Task Consume(ConsumeContext<BasketInfoRequest> context)
        {
            var basket = await _repo.GetBasketAsync(context.Message.UserId);
            if (basket == null)
            {
                throw new InvalidOperationException("Basket not found");
            }

            await context.RespondAsync<BasketInfoResponse>(new BasketInfoResponse
            {
                Items = basket.Items.Select(x => new OrderItem
                {
                    Price = x.Price,
                    Quantity = x.Quantity,
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                }),
                UserId = basket.UserName,
                Price = basket.TotalPrice,
            });
        }
    }
}
