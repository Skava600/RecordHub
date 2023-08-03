using Bogus;
using RecordHub.OrderingService.Domain.Entities;
using RecordHub.Shared.Enums;

namespace RecordHub.OrderingService.Tests.Generators
{
    public class OrderGenerator : Faker<Order>
    {
        public OrderGenerator()
        {
            RuleFor(o => o.Id, f => Guid.NewGuid());
            RuleFor(o => o.UserId, f => f.Random.Guid().ToString());
            RuleFor(o => o.FirstName, f => f.Person.FirstName);
            RuleFor(o => o.LastName, f => f.Person.LastName);
            RuleFor(o => o.EmailAddress, f => f.Person.Email);
            RuleFor(o => o.TotalPrice, f => f.Random.Double(100, 1000));
            RuleFor(o => o.Address, f => f.Address.FullAddress());
            RuleFor(o => o.State, f => f.PickRandom<StatesEnum>());
            RuleFor(o => o.Items, new OrderItemGenerator().Generate(3));
        }
    }
}
