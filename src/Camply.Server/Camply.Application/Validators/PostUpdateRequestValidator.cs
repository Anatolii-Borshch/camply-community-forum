using Camply.Shared.Dtos.Post;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class PostUpdateRequestValidator : AbstractValidator<PostUpdateRequest>
    {
        public PostUpdateRequestValidator()
        {
            RuleFor(x => x.PostId)
                .NotEmpty().WithMessage("PostId is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must be less than 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must be less than 2000 characters.");

            RuleFor(x => x.AuthorId)
                .NotEmpty().WithMessage("AuthorId is required.");
        }
    }
}