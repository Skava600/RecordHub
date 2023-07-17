using FluentValidation;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Application.Validators
{
    public class BasketItemValidator : AbstractValidator<BasketItem>
    {
        public BasketItemValidator()
        {
            RuleFor(x => x.Price)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Price is a double value greater than zero");

            RuleFor(x => x.Quantity)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Quantity is a integer value greater than zero");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .Must(p => Guid.TryParse(p, out _))
                .WithMessage("Product id is guid");

            RuleFor(x => x.ProductName).NotEmpty();
        }
    }
}
