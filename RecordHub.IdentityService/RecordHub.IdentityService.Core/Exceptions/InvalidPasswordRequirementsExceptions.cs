using Microsoft.AspNetCore.Identity;

namespace RecordHub.IdentityService.Core.Exceptions
{
    public class InvalidPasswordRequirementsExceptions : IdentityErrorsException
    {
        public const string InvalidPasswordCriteria = "Password  does not meet the criteria";

        public InvalidPasswordRequirementsExceptions(IEnumerable<IdentityError> errors)
            : base(errors, InvalidPasswordCriteria)
        {
        }
    }
}
