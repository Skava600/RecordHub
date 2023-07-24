using Bogus;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Tests.Generators
{
    public class UserGenerator
    {
        private Faker<RegisterModel> _fakerRegisterModel;
        private AddressGenerator _addressGenerator;

        public UserGenerator()
        {
            _addressGenerator = new AddressGenerator();

            _fakerRegisterModel = new Faker<RegisterModel>()
                .RuleFor(r => r.Name, f => f.Name.FirstName())
                .RuleFor(r => r.Surname, f => f.Name.LastName())
                .RuleFor(r => r.Password, f => f.Internet.Password())
                .RuleFor(r => r.Email, f => f.Internet.Email())
                .RuleFor(r => r.PhoneNumber, f => f.Person.Phone);
        }

        public RegisterModel GenerateRegisterModel() => _fakerRegisterModel.Generate();

        public UserDTO GenerateUserDTO()
        {
            var model = GenerateRegisterModel();

            var user = new UserDTO
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Surname = model.Surname,
                Addresses = new List<Address>
                {
                    _addressGenerator.Generate(),
                    _addressGenerator.Generate(),
                }
            };

            return user;
        }
    }
}
