using System.Text;
using Camply.Application.Contracts.Repositories;
using Camply.Shared.Dtos.Forum;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class ForumUpdateRequestValidator : AbstractValidator<ForumUpdateRequest>
    {
        private readonly IForumRepository _forumRepository;

        public ForumUpdateRequestValidator(IForumRepository forumRepository)
        {
            _forumRepository = forumRepository;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters")
                .Must(title => !title.All(char.IsLetter))
                    .WithMessage("Title cannot consist only of letters")
                .MustAsync(async (request, title, cancellation) => 
                    !await TitleExistsAsync(request.Id, title))
                    .WithMessage("A forum with this title already exists");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Tags)
                .NotNull()
                .Must(tags => tags.Count <= 10)
                .WithMessage("Maximum 10 tags allowed");

            RuleFor(x => x.AdminId)
                .NotEmpty().WithMessage("Admin is required")
                .MustAsync(async (request, adminId, cancellation) => 
                    await IsAdminOfForum(request.Id, adminId))
                .WithMessage("User is not the admin of this forum");
        }

        private async Task<bool> TitleExistsAsync(Guid forumId, string title)
        {
            var normalizedTitle = title.Trim().ToLower();
            var existingForums = await _forumRepository.GetAllAsync();
            return existingForums.Any(f => f.Id != forumId && f.Title.ToLower() == normalizedTitle);
        }

        private async Task<bool> IsAdminOfForum(Guid forumId, Guid adminId)
        {
            var forum = await _forumRepository.GetByIdAsync(forumId);
            if (forum == null) return false;
            return forum.AdminId == adminId;
        }
    }
}