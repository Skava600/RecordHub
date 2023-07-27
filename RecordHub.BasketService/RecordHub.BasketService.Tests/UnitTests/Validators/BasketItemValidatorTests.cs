using FluentValidation;
using FluentValidation.TestHelper;
using RecordHub.BasketService.Application.Validators;
using RecordHub.BasketService.Domain.Entities;

namespace RecordHub.BasketService.Tests.UnitTests.Validators
{
    public class BasketItemValidatorTests
    {
        private readonly IValidator<BasketItem> _validator;

        public BasketItemValidatorTests()
        {
            _validator = new BasketItemValidator();
        }

        [Theory]
        [MemberData(nameof(GetInvalidPrices))]
        public void Validate_InvalidPrice_ShouldFailValidation(double price)
        {
            // Arrange
            var basketItem = new BasketItem
            {
                Price = price,
                Quantity = 2,
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "Product"
            };

            // Act
            var result = _validator.TestValidate(basketItem);

            // Assert
            result.ShouldHaveValidationErrorFor(item => item.Price)
                .WithErrorMessage("Price is a double value greater than zero");
        }

        [Theory]
        [MemberData(nameof(GetInvalidQuantities))]
        public void Validate_InvalidQuantity_ShouldFailValidation(int quantity)
        {
            // Arrange
            var basketItem = new BasketItem
            {
                Price = 20.99,
                Quantity = quantity,
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "Product"
            };

            // Act
            var result = _validator.TestValidate(basketItem);

            // Assert
            result.ShouldHaveValidationErrorFor(item => item.Quantity)
                .WithErrorMessage("Quantity is a integer value greater than zero");
        }

        [Theory]
        [InlineData("invalidGuid")]
        [InlineData("")]
        public void Validate_InvalidProductId_ShouldFailValidation(string productId)
        {
            // Arrange
            var basketItem = new BasketItem
            {
                Price = 20.99,
                Quantity = 2,
                ProductId = productId,
                ProductName = "Product"
            };

            // Act
            var result = _validator.TestValidate(basketItem);

            // Assert
            result.ShouldHaveValidationErrorFor(item => item.ProductId);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Validate_EmptyProductName_ShouldFailValidation(string productName)
        {
            // Arrange
            var basketItem = new BasketItem
            {
                Price = 20.99,
                Quantity = 2,
                ProductId = Guid.NewGuid().ToString(),
                ProductName = productName
            };

            // Act
            var result = _validator.TestValidate(basketItem);

            // Assert
            result.ShouldHaveValidationErrorFor(item => item.ProductName)
                .WithErrorMessage("'Product Name' must not be empty.");
        }

        [Fact]
        public void Validate_ValidBasketItem_ShouldPassValidation()
        {
            // Arrange
            var basketItem = new BasketItem
            {
                Price = 20.99,
                Quantity = 2,
                ProductId = Guid.NewGuid().ToString(),
                ProductName = "Product"
            };

            // Act
            var result = _validator.TestValidate(basketItem);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        public static IEnumerable<object[]> GetInvalidPrices()
        {
            yield return new object[] { -1 };
            yield return new object[] { -5.99 };
        }

        public static IEnumerable<object[]> GetInvalidQuantities()
        {
            yield return new object[] { -199 };
            yield return new object[] { -1 };
        }
    }
}
