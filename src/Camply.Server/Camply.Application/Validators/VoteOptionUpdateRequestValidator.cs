using Camply.Shared.Dtos.Vote;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class VoteOptionUpdateRequestValidator : AbstractValidator<VoteOptionUpdateRequest>
    {
        public VoteOptionUpdateRequestValidator()
        {
            RuleFor(x => x.VoteOptionId).NotEmpty();
            RuleFor(x => x.AuthorId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Index).GreaterThanOrEqualTo(0);
        }
    }
}