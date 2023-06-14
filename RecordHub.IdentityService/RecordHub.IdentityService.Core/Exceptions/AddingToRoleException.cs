using Microsoft.AspNetCore.Identity;

namespace RecordHub.IdentityService.Core.Exceptions
{
    public class AddingToRoleException : MultipleErrorException
    {
        public const string AddingToRoleExceptionMessage = "Error when adding to role";
        public AddingToRoleException(IEnumerable<IdentityError> errors) : base(errors, AddingToRoleExceptionMessage)
        {
        }
    }
}
