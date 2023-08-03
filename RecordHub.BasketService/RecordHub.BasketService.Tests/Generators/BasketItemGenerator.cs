using Bogus;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Tests.Generators
{
    public static class BasketItemGenerator
    {
        private static readonly Faker<BasketItem> _faker;

        static BasketItemGenerator()
        {
            _faker = new Faker<BasketItem>()
                .RuleFor(item => item.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(item => item.Price, f => f.Random.Double(10, 100))
                .RuleFor(item => item.ProductId, f => f.Random.Guid().ToString())
                .RuleFor(item => item.ProductName, f => f.Commerce.ProductName());
        }

        public static BasketItem Generate()
        {
            return _faker.Generate();
        }

        public static IEnumerable<BasketItem> GenerateBeetween(int min, int max)
        {
            return _faker.GenerateBetween(min, max);
        }
    }
}
