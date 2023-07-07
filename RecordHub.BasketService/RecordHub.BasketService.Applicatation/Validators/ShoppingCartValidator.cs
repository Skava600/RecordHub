using FluentValidation;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Applicatation.Validators
{
    public class ShoppingCartValidator : AbstractValidator<ShoppingCart>
    {
        public ShoppingCartValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().Matches("^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$").WithMessage("UserName is guid");
        }

    }
}
