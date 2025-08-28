using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos;
using Camply.Shared.Dtos.Forum;
using FluentValidation;

namespace Camply.Application.Implementations
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;
        private readonly IValidator<ForumCreateRequest> _createValidator;
        private readonly IValidator<ForumUpdateRequest> _updateValidator;
        
        public ForumService(IForumRepository forumRepository, IValidator<ForumCreateRequest> createValidator
            , IValidator<ForumUpdateRequest> updateValidator)
        {
            _forumRepository = forumRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }
        
        public async Task<ForumDto> GetForumById(Guid id)
        {
            Forum? searchedForum = await _forumRepository.GetByIdAsync(id);

            if (searchedForum == null)
                throw new KeyNotFoundException($"Forum with id {id} not found.");

            ForumDto mappedForum = searchedForum.MapToForumDto();
            return mappedForum;
        }

        public async Task<IEnumerable<ForumPrevievDto>> GetAllForums(ForumSearchRequest request)
        {
            var spec = new ForumSearchSpecification(request);

            var forums = await _forumRepository.ListAsync(spec);

            var pagedForums = forums
                .Skip(request.Skip)
                .Take(request.Take)
                .ToList();

            var mappedForums = pagedForums.Select(x => x.MapToForumPreviewDto());
            return mappedForums;
        }

        public async Task CreateForum(ForumCreateRequest request)
        {
            // Validate request
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var forum = new Forum
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                AdminId = request.AdminId,
                CreatedDate = DateTime.UtcNow,
                Tags = request.Tags.Select(t => new Tag { Name = t.Name }).ToList()
            };

            await _forumRepository.AddAsync(forum);
        }

        public async Task UpdateForum(ForumUpdateRequest request)
        {
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var forum = await _forumRepository.GetByIdAsync(request.Id);
            if (forum == null)
                throw new KeyNotFoundException($"Forum with id {request.Id} not found.");
            
            forum.Title = request.Title;
            forum.Description = request.Description;

            forum.Tags.Clear();
            forum.Tags = request.Tags.Select(t => new Tag { Name = t.Name }).ToList();

            await _forumRepository.UpdateAsync(forum);
        }

        public async Task DeleteForum(Guid id)
        {
            var forum = await _forumRepository.GetByIdAsync(id);
            
            if(forum == null)
                throw new KeyNotFoundException($"Forum with id {id} not found.");
            
            await _forumRepository.DeleteAsync(forum);
        }
    }
}