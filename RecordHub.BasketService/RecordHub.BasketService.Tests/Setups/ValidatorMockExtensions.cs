using FluentValidation;
using FluentValidation.Results;

namespace RecordHub.BasketService.Tests.Setups
{
    internal static class ValidatorMockExtensions
    {
        public static void SetupValidatorMock<TEntity>(this Mock<IValidator<TEntity>> validatorMock, ValidationResult validResult, CancellationToken cancellationToken = default)
             where TEntity : class
        {
            validatorMock
                .Setup(_ => _.ValidateAsync(It.IsAny<TEntity>(), cancellationToken))
                .ReturnsAsync(validResult)
                .Verifiable();
        }

        public static void SetupValidatorMockThrowsException<TEntity>(this Mock<IValidator<TEntity>> validatorMock, ValidationException ex, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            validatorMock
               .Setup(v => v.ValidateAsync(It.IsAny<TEntity>(), cancellationToken))
               .Throws(ex);
        }
    }
}
