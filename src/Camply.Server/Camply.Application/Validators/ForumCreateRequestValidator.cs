using Camply.Application.Contracts.Repositories;
using Camply.Shared.Dtos.Forum;
using FluentValidation;

namespace Camply.Application.Validators
{
    public class ForumCreateRequestValidator : AbstractValidator<ForumCreateRequest>
    {
        private readonly IForumRepository _forumRepository;
        private readonly IUserRepository _userRepository;

        public ForumCreateRequestValidator(IForumRepository forumRepository, IUserRepository userRepository)
        {
            _forumRepository = forumRepository;
            _userRepository = userRepository;

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MinimumLength(5).WithMessage("Title must be at least 5 characters")
                .MaximumLength(200).WithMessage("Title must be at most 200 characters")
                .MustAsync(async (title, cancellation) => !await TitleExistsAsync(title))
                .WithMessage("A forum with this title already exists");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

            RuleFor(x => x.Tags)
                .NotNull()
                .Must(tags => tags.Count <= 10).WithMessage("Maximum 10 tags allowed");

            RuleFor(x => x.AdminId)
                .NotEmpty().WithMessage("Admin is required")
                .MustAsync(async (adminId, cancellation) => await AdminExistsAsync(adminId))
                .WithMessage("Admin user does not exist");
        }

        private async Task<bool> TitleExistsAsync(string title)
        {
            var normalizedTitle = title.Trim().ToLower();
            var existingForums = await _forumRepository.GetAllAsync();
            return existingForums.Any(f => f.Title.ToLower() == normalizedTitle);
        }

        private async Task<bool> AdminExistsAsync(Guid adminId)
        {
            var admin = await _userRepository.GetByIdAsync(adminId);
            return admin != null;
        }
    }
}