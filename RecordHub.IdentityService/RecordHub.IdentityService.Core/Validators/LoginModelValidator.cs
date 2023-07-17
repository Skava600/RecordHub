using FluentValidation;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Core.Validators
{
    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(l => l.UserName).NotEmpty();

            RuleFor(l => l.Password).NotEmpty();
        }
    }
}
