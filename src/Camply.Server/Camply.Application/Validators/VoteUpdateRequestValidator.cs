using Camply.Shared.Dtos.Vote;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class VoteUpdateRequestValidator : AbstractValidator<VoteUpdateRequest>
    {
        public VoteUpdateRequestValidator()
        {
            RuleFor(x => x.VoteId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        }
    }
}