using Camply.Shared.Dtos.Vote;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class VoteOptionCreateDtoValidator : AbstractValidator<VoteOptionCreateDto>
    {
        public VoteOptionCreateDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        }
    }
}