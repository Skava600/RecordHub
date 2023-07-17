using FluentValidation;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Core.Validators
{
    public class RegisterModelValidator : AbstractValidator<RegisterModel>
    {
        public RegisterModelValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotEmpty()
                .Length(5, 40);

            RuleFor(r => r.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\+375\d{9}$")
                .WithMessage("Invalid Belarus phone number");

            RuleFor(r => r.Name)
                .NotEmpty()
                .Length(5, 40)
                .Matches("^[a-zA-Zа-яА-Я]+")
                .WithMessage("Name consists only of latin or cyrillic letters");

            RuleFor(r => r.Surname)
                .NotEmpty()
                .Length(5, 40)
                .Matches("^[a-zA-Zа-яА-Я]+")
                .WithMessage("Surname consists only of latin or cyrillic letters");
        }
    }
}
