using Microsoft.AspNetCore.Identity;

namespace RecordHub.IdentityService.Core.Exceptions
{
    public class ConfirmationEmailException : IdentityErrorsException
    {
        private const string ErrorValidatingToken = "Error during valtidation of token";
        public ConfirmationEmailException(IEnumerable<IdentityError> Errors) : base(Errors, ErrorValidatingToken)
        {
        }
    }
}
