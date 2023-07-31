using Bogus;
using RecordHub.OrderingService.Domain.Entities;

namespace RecordHub.OrderingService.Tests.Generators
{
    public class OrderItemGenerator : Faker<OrderItem>
    {
        public OrderItemGenerator()
        {
            RuleFor(o => o.Quantity, f => f.Random.Int(1, 10));
            RuleFor(o => o.Price, f => f.Random.Double(10, 100));
            RuleFor(o => o.ProductId, f => Guid.NewGuid().ToString());
            RuleFor(o => o.ProductName, f => f.Commerce.ProductName());
        }
    }
}
