using Bogus;
using RecordHub.BasketService.Domain.Models;

namespace RecordHub.BasketService.Tests.Generators
{
    public static class BasketCheckoutModelGenerator
    {
        private static readonly Faker<BasketCheckoutModel> _faker;

        static BasketCheckoutModelGenerator()
        {
            _faker = new Faker<BasketCheckoutModel>()
                .RuleFor(m => m.UserId, f => f.Random.Guid().ToString())
                .RuleFor(m => m.FirstName, f => f.Person.FirstName)
                .RuleFor(m => m.LastName, f => f.Person.LastName)
                .RuleFor(m => m.EmailAddress, f => f.Person.Email)
                .RuleFor(m => m.Address, f => f.Address.FullAddress())
                .RuleFor(m => m.PhoneNumber, f => f.Phone.PhoneNumber());
        }

        public static BasketCheckoutModel GenerateModel()
        {
            return _faker.Generate();
        }
    }
}
