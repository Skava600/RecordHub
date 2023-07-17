using FluentValidation;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Application.Validators
{
    public class BaseValidator : AbstractValidator<BaseEntity>
    {
        public BaseValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(x => x.Slug)
                .NotEmpty()
                .Matches($"^[a-z-]+$")
                .WithMessage("Slug is latin string with length from 2 to 100")
                .Length(2, 100);
        }
    }
}
