using Bogus;
using RecordHub.Shared.MassTransit.Models.Order;

namespace RecordHub.OrderingService.Tests.Generators
{
    internal class BasketCheckoutMessageGenerator : Faker<BasketCheckoutMessage>
    {
        public BasketCheckoutMessageGenerator()
        {
            RuleFor(m => m.UserId, f => f.Random.Guid().ToString())
            .RuleFor(m => m.FirstName, f => f.Person.FirstName)
            .RuleFor(m => m.LastName, f => f.Person.LastName)
            .RuleFor(m => m.EmailAddress, f => f.Person.Email)
            .RuleFor(m => m.Address, f => f.Address.FullAddress())
            .RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(m => m.Items, f =>
            {
                var items = new OrderItemGenerator().GenerateBetween(3, 10);
                return items.Select(i => new OrderItemModel
                {
                    Price = i.Price,
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity
                });
            });
        }
    }
}
