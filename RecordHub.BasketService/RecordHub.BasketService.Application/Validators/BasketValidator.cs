using FluentValidation;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Application.Validators
{
    public class BasketValidator : AbstractValidator<Basket>
    {
        public BasketValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .Must(username => Guid.TryParse(username, out _))
                .WithMessage("UserName is guid");
        }
    }
}
