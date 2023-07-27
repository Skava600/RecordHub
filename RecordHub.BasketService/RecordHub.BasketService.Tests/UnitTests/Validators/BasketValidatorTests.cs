using FluentValidation;
using FluentValidation.TestHelper;
using RecordHub.BasketService.Application.Validators;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Tests.UnitTests.Validators
{
    public class BasketValidatorTests
    {
        private readonly IValidator<Basket> _validator;

        public BasketValidatorTests()
        {
            _validator = new BasketValidator();
        }

        [Theory]
        [MemberData(nameof(GetInvalidUserNames))]
        public void Validate_InvalidUserName_ShouldFailValidation(string userName)
        {
            // Arrange
            var basket = new Basket
            {
                UserName = userName
            };

            // Act
            var result = _validator.TestValidate(basket);

            // Assert
            result.ShouldHaveValidationErrorFor(b => b.UserName)
                .WithErrorMessage("'User Name' must not be empty.");
        }

        [Theory]
        [MemberData(nameof(GetInvalidUserNameFormats))]
        public void Validate_InvalidUserNameFormat_ShouldFailValidation(string userName)
        {
            // Arrange
            var basket = new Basket
            {
                UserName = userName
            };

            // Act
            var result = _validator.TestValidate(basket);

            // Assert
            result.ShouldHaveValidationErrorFor(b => b.UserName)
                .WithErrorMessage("'User Name' must be a valid GUID.");
        }

        [Fact]
        public void Validate_ValidBasket_ShouldPassValidation()
        {
            // Arrange
            var basket = new Basket
            {
                UserName = Guid.NewGuid().ToString()
            };

            // Act
            var result = _validator.TestValidate(basket);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        public static IEnumerable<object[]> GetInvalidUserNames()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
        }

        public static IEnumerable<object[]> GetInvalidUserNameFormats()
        {
            yield return new object[] { "invalidGuid" };
            yield return new object[] { "invalidUserName" };
        }
    }
}
