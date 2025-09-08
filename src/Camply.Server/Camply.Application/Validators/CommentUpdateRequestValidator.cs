using Camply.Shared.Dtos.Comment;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class CommentUpdateRequestValidator : AbstractValidator<CommentUpdateRequest>
    {
        public CommentUpdateRequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}