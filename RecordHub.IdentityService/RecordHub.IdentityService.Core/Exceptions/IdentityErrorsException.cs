using Microsoft.AspNetCore.Identity;

namespace RecordHub.IdentityService.Core.Exceptions
{
    public abstract class IdentityErrorsException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; set; }
        public IdentityErrorsException(IEnumerable<IdentityError> Errors, string message) : base(message)
        {
            this.Errors = Errors;
        }
    }
}
