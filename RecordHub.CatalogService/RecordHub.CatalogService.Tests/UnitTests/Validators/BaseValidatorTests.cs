using FluentValidation.TestHelper;
using RecordHub.CatalogService.Application.Validators;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Tests.UnitTests.Validators
{
    public class BaseValidatorTests
    {
        private readonly BaseValidator _validator;

        public BaseValidatorTests()
        {
            _validator = new BaseValidator();
        }

        [Theory]
        [MemberData(nameof(InvalidNames))]
        public void Name_WhenInvalid_ShouldHaveValidationError(string name)
        {
            // Act
            var result = _validator.TestValidate(new BaseEntity { Name = name });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        public static IEnumerable<object[]> InvalidNames => new List<object[]>
        {
            new object[] { null },
            new object[] { "" },
            new object[] { " " },
            new object[] { "a" },
            new object[] { "a".PadRight(101, 'a') }
        };

        [Theory]
        [MemberData(nameof(InvalidSlugs))]
        public void Slug_WhenInvalid_ShouldHaveValidationError(string slug)
        {
            // Act
            var result = _validator.TestValidate(new BaseEntity { Slug = slug });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Slug);
        }

        public static IEnumerable<object[]> InvalidSlugs => new List<object[]>
        {
            new object[] { null },
            new object[] { "" },
            new object[] { " " },
            new object[] { "invalid slug" },
            new object[] { "a" },
            new object[] { "a".PadRight(101, 'a') }
        };
    }
}