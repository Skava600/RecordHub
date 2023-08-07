using Bogus;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Tests.Generators
{
    public class AddressGenerator
    {
        private Faker<Address> _fakerAddress;

        public AddressGenerator()
        {
            _fakerAddress = new Faker<Address>()
                .RuleFor(a => a.UserId, f => f.Random.Uuid())
                .RuleFor(a => a.Id, f => f.Random.Uuid())
                .RuleFor(a => a.Korpus, f => f.Random.AlphaNumeric(1).ToUpper())
                .RuleFor(a => a.Street, f => f.Address.StreetAddress())
                .RuleFor(a => a.State, f => f.Address.State())
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.Appartment, f => f.Random.Int(1, 999).ToString())
                .RuleFor(a => a.HouseNumber, f => f.Random.Int(1, 999).ToString())
                .RuleFor(a => a.Postcode, f => f.Address.ZipCode("######"));
        }

        public Address Generate() => _fakerAddress.Generate();

        public AddressModel GenerateModel()
        {
            var address = this.Generate();

            return new AddressModel
            {
                Appartment = address.Appartment,
                City = address.City,
                HouseNumber = address.HouseNumber,
                Korpus = address.Korpus,
                State = address.State,
                Street = address.Street,
                Postcode = address.Postcode,
            };
        }
    }
}
