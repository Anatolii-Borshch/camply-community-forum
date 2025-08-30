using Camply.Shared.Dtos.User;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class AccountDeleteRequestValidator : AbstractValidator<AccountDeleteRequest>
    {
        public AccountDeleteRequestValidator()
        {
            RuleFor(r => r.UserId).NotEmpty();
            RuleFor(r => r.Password).NotEmpty();
        }
    }
}