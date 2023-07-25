using FluentValidation.TestHelper;
using RecordHub.CatalogService.Application.Validators;
using RecordHub.CatalogService.Domain.Entities;
using Record = RecordHub.CatalogService.Domain.Entities.Record;

namespace RecordHub.CatalogService.Tests.UnitTests.Validators
{
    public class RecordValidatorTests
    {
        private readonly RecordValidator _validator;

        public RecordValidatorTests()
        {
            _validator = new RecordValidator();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Name_WhenNullOrEmptyOrWhitespace_ShouldHaveValidationError(string name)
        {
            // Act
            var result = _validator.TestValidate(new Record { Name = name });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [MemberData(nameof(InvalidSlugs))]
        public void Slug_WhenInvalid_ShouldHaveValidationError(string slug)
        {
            // Act
            var result = _validator.TestValidate(new Record { Slug = slug });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Slug);
        }

        [Theory]
        [MemberData(nameof(ValidSlugs))]
        public void Slug_WhenValid_ShouldNotHaveValidationError(string slug)
        {
            // Act
            var result = _validator.TestValidate(new Record { Slug = slug });

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Slug);
        }

        [Theory]
        [MemberData(nameof(GetInvalidNameData))]
        public void Name_WhenInvalid_ShouldHaveValidationError(string name)
        {
            // Act
            var result = _validator.TestValidate(new Record { Name = name });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Name);
        }


        [Theory]
        [InlineData("Sample Name")]
        public void Name_WhenValid_ShouldNotHaveValidationError(string name)
        {
            // Act
            var result = _validator.TestValidate(new Record { Name = name });

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [MemberData(nameof(GetInvalidNameData))]
        public void Description_WhenInvalid_ShouldHaveValidationError(string description)
        {
            // Act
            var result = _validator.TestValidate(new Record { Description = description });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData("Sample Description")]
        public void Description_WhenValid_ShouldNotHaveValidationError(string description)
        {
            // Act
            var result = _validator.TestValidate(new Record { Description = description });

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-10)]
        public void Radius_WhenInvalid_ShouldHaveValidationError(short radius)
        {
            // Act
            var result = _validator.TestValidate(new Record { Radius = radius });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Radius);
        }

        // Test for Price property
        [Theory]
        [InlineData(null)]
        [InlineData(0)]
        [InlineData(-10)]
        public void Price_WhenInvalid_ShouldHaveValidationError(double price)
        {
            // Act
            var result = _validator.TestValidate(new Record { Price = price });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Price);
        }

        [Fact]
        public void Styles_WhenNullOrEmpty_ShouldHaveValidationError()
        {
            // Act
            var result1 = _validator.TestValidate(new Record { Styles = null });
            var result2 = _validator.TestValidate(new Record { Styles = new List<Style>() });

            // Assert
            result1.ShouldHaveValidationErrorFor(x => x.Styles);
            result2.ShouldHaveValidationErrorFor(x => x.Styles);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(1920)]
        public void Year_WhenInvalid_ShouldHaveValidationError(int year)
        {
            // Act
            var result = _validator.TestValidate(new Record { Year = year });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Year);
        }

        [Fact]
        public void ArtistId_WhenEmpty_ShouldHaveValidationError()
        {
            // Act
            var result = _validator.TestValidate(new Record { ArtistId = Guid.Empty });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ArtistId);
        }

        [Fact]
        public void Country_WhenEmpty_ShouldHaveValidationError()
        {
            // Act
            var result = _validator.TestValidate(new Record { Country = null });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Country);
        }

        [Fact]
        public void Label_WhenEmpty_ShouldHaveValidationError()
        {
            // Act
            var result = _validator.TestValidate(new Record { Label = null });

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Label);
        }

        public static IEnumerable<object[]> InvalidSlugs()
        {
            return new List<object[]>
            {
                new object[] { null },
                new object[] { "" },
                new object[] { " " },
                new object[] { "a" },
                new object[] { "a".PadRight(101, 'a') },
                new object[] { "123" }
            };
        }

        public static IEnumerable<object[]> GetInvalidNameData()
        {
            return new List<object[]>
            {
                new object[] { null },
                new object[] { "" },
                new object[] { " " },
                new object[] { "a" },
                new object[] { "a".PadRight(101, 'a') },
            };
        }

        public static IEnumerable<object[]> ValidSlugs()
        {
            return new List<object[]>
            {
                new object[] { "valid-slug" }
            };
        }
    }
}
