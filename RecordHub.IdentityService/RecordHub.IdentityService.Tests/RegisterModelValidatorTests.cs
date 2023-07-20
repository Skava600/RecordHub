using FluentValidation.TestHelper;
using RecordHub.IdentityService.Core.Validators;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Tests
{
    public class RegisterModelValidatorTests
    {
        private readonly RegisterModelValidator _validator;

        public RegisterModelValidatorTests()
        {
            _validator = new RegisterModelValidator();
        }

        [Fact]
        public void Validate_ValidModel_ShouldNotHaveValidationError()
        {
            // Arrange
            var model = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "Johny",
                Surname = "Doeban"
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_InvalidEmail_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "invalidemail",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "John",
                Surname = "Doe"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Email)
                .WithErrorMessage("'Email' is not a valid email address.");
        }

        [Fact]
        public void Validate_InvalidPhoneNumber_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "1234567890",
                Name = "John",
                Surname = "Doe"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.PhoneNumber)
                .WithErrorMessage("Invalid Belarus phone number");
        }

        [Fact]
        public void Validate_EmptyEmail_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "John",
                Surname = "Doe"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Email)
                .WithErrorMessage("'Email' must not be empty.");
        }

        [Fact]
        public void Validate_ShortPassword_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "123",
                PhoneNumber = "+375291234567",
                Name = "John",
                Surname = "Doe"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Password)
                .WithErrorMessage("'Password' must be between 5 and 40 characters. You entered 3 characters.");
        }

        [Fact]
        public void Validate_EmptyName_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "",
                Surname = "Doe"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Name)
                .WithErrorMessage("'Name' must not be empty.");
        }

        [Fact]
        public void Validate_EmptySurname_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "John",
                Surname = ""
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Surname)
                .WithErrorMessage("'Surname' must not be empty.");
        }

        [Fact]
        public void Validate_ShortName_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "J",
                Surname = "Doe"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Name)
                .WithErrorMessage("'Name' must be between 5 and 40 characters. You entered 1 characters.");
        }

        [Fact]
        public void Validate_ShortSurname_ShouldFailValidation()
        {
            // Arrange
            var registerModel = new RegisterModel
            {
                Email = "test@example.com",
                Password = "password123",
                PhoneNumber = "+375291234567",
                Name = "John",
                Surname = "D"
            };

            // Act
            var result = _validator.TestValidate(registerModel);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Surname)
                .WithErrorMessage("'Surname' must be between 5 and 40 characters. You entered 1 characters.");
        }
    }
}
