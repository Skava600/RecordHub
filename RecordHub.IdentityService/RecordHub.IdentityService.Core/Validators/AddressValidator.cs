using FluentValidation;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Core.Validators
{
    public class AddressValidator : AbstractValidator<AddressModel>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Postcode)
                .NotEmpty()
                .Matches("^[0-9]{6}$")
                .WithMessage("Postcode is a 6 digit number");

            RuleFor(x => x.Street)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(x => x.City)
                .NotEmpty()
                .Length(2, 100);
            RuleFor(x => x.State)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(x => x.Appartment)
                .Matches("^[0-9]{0,3}$")
                .WithMessage("Appartment is number between 1-999");

            RuleFor(x => x.Korpus)
                .Matches("^[0-9А-Я]{0,1}")
                .WithMessage("Korpus is a digit or a character");

            RuleFor(x => x.HouseNumber)
                .Matches("^[0-9]{1,3}")
                .WithMessage("House number is a number between 1-999");
        }
    }
}
