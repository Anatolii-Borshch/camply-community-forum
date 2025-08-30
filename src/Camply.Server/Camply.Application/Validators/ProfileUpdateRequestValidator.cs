using Camply.Shared.Dtos.User;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class ProfileUpdateRequestValidator : AbstractValidator<ProfileUpdateRequest>
    {
        public ProfileUpdateRequestValidator()
        {
            RuleFor(r => r.Name).NotEmpty().MaximumLength(50);
            RuleFor(r => r.Surname).NotEmpty().MaximumLength(50);
            RuleFor(r => r.Birthday).LessThan(DateTime.UtcNow);
            RuleFor(r => r.Username).NotEmpty().MaximumLength(30);
        }
    }
}