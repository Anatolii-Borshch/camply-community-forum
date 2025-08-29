using Camply.Shared.Dtos.Vote;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class VoteCreateRequestValidator : AbstractValidator<VoteCreateRequest>
    {
        public VoteCreateRequestValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
            RuleFor(x => x.ForumId).NotEmpty();
            RuleFor(x => x.AuthorId).NotEmpty();
            RuleFor(x => x.VoteOptions).NotEmpty().WithMessage("At least one option is required");
            RuleForEach(x => x.VoteOptions).SetValidator(new VoteOptionCreateDtoValidator());
        }
    }
}