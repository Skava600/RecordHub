using FluentValidation;
using FluentValidation.Results;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Tests.Setups
{
    internal static class ValidatorMockExtensions
    {
        public static void SetupValidatorMock<TEntity>(this Mock<IValidator<TEntity>> validatorMock, ValidationResult validResult, CancellationToken cancellationToken = default)
             where TEntity : BaseEntity
        {
            validatorMock
                .Setup(_ => _.ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
                .ReturnsAsync(validResult)
                .Verifiable();
        }

        public static void SetupValidatorMockThrowsException<TEntity>(this Mock<IValidator<TEntity>> validatorMock, ValidationException ex, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity
        {
            validatorMock
               .Setup(v => v.ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
               .Throws(ex);
        }
    }
}
