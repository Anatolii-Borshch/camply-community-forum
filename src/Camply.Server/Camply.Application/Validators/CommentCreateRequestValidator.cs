using Camply.Shared.Dtos.Comment;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class CommentCreateRequestValidator : AbstractValidator<CommentCreateRequest>
    {
        public CommentCreateRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PostId).NotEmpty();
            RuleFor(x => x.Content)
                .NotEmpty()
                .MaximumLength(500);
        }
    }
}