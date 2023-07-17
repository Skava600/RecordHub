using FluentValidation;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Application.Validators
{
    public class RecordValidator : AbstractValidator<Record>
    {
        public RecordValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(x => x.Description)
                .NotEmpty()
                .Length(2, 100);

            RuleFor(x => x.Radius)
                .NotEmpty()
                .Must(x => x > 0)
                .WithMessage("Radius should be greater than zero.");

            RuleFor(x => x.Price
            ).NotEmpty()
            .GreaterThan(0);

            RuleFor(x => x.Slug)
                .NotEmpty()
                .Matches($"^[a-z-]+$")
                .WithMessage("Slug is latin string with length from 2 to 100")
                .Length(2, 100);

            RuleFor(x => x.Year)
                .NotEmpty()
                .GreaterThan(1930);

            RuleFor(x => x.Styles).NotEmpty();

            RuleFor(x => x.ArtistId).NotEmpty();

            RuleFor(x => x.Country).NotEmpty();

            RuleFor(x => x.Label).NotEmpty();
        }
    }
}
