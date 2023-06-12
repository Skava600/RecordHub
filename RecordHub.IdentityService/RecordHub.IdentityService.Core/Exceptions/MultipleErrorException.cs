using Microsoft.AspNetCore.Identity;

namespace RecordHub.IdentityService.Core.Exceptions
{
    public abstract class MultipleErrorException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; set; }
        public MultipleErrorException(IEnumerable<IdentityError> Errors, string message) : base(message)
        {
            this.Errors = Errors;
        }
    }
}
