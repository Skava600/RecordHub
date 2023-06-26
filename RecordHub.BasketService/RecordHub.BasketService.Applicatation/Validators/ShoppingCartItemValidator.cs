using FluentValidation;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Applicatation.Validators
{
    public class ShoppingCartItemValidator : AbstractValidator<ShoppingCartItem>
    {
        public ShoppingCartItemValidator()
        {
            RuleFor(x => x.Price).NotEmpty().GreaterThan(0).WithMessage("Price is a double value greater than zero");
            RuleFor(x => x.Quantity).NotEmpty().GreaterThan(0).WithMessage("Quantity is a integer value greater than zero");
            RuleFor(x => x.ProductId).NotEmpty().Matches("^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$").WithMessage("Product id is guid");
            RuleFor(x => x.ProductName).NotEmpty();
        }
    }
}
