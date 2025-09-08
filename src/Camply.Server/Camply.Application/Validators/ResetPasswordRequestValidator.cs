using Camply.Shared.Dtos.User;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(r => r.UserId).NotEmpty();
            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(6);
        }
    }
}