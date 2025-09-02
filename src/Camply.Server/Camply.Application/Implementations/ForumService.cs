using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Application.Mappers;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Shared.Dtos;
using Camply.Shared.Dtos.Forum;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _forumRepository;
        private readonly IValidator<ForumCreateRequest> _createValidator;
        private readonly IValidator<ForumUpdateRequest> _updateValidator;
        private readonly ISpecifiedRepository<Forum> _specifiedRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ILogger<ForumService> _logger;

        public ForumService(IForumRepository forumRepository, IValidator<ForumCreateRequest> createValidator
            , IValidator<ForumUpdateRequest> updateValidator, ISpecifiedRepository<Forum> specifiedRepository
            , ITagRepository tagRepository, ILogger<ForumService> logger)
        {
            _forumRepository = forumRepository;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _specifiedRepository = specifiedRepository;
            _tagRepository = tagRepository;
            _logger = logger;
        }
        
        public async Task<ForumDto> GetForumById(Guid id)
        {
            _logger.LogInformation("Fetching forum with id {ForumId}", id);
            
            Forum? searchedForum = await _forumRepository.GetByIdAsync(id);

            if (searchedForum == null)
            {
                _logger.LogWarning("Forum with id {ForumId} not found", id);
                throw new KeyNotFoundException($"Forum with id {id} not found.");
            }

            ForumDto mappedForum = searchedForum.MapToForumDto();
            _logger.LogInformation("Forum {ForumId} fetched successfully", id);
            
            return mappedForum;
        }

        public async Task<IEnumerable<ForumPrevievDto>> GetAllForums(ForumSearchRequest request)
        {
            _logger.LogInformation("Fetching all forums with search criteria {@Request}", request);
            
            var spec = new ForumSearchSpecification(request);
            var forums = await _specifiedRepository.ListAsync(spec);

            var mappedForums = forums.Select(x => x.MapToForumPreviewDto());
            _logger.LogInformation("Fetched {Count} forums", mappedForums.Count());
            
            return mappedForums;
        }

        public async Task CreateForum(ForumCreateRequest request)
        {
            _logger.LogInformation("Creating forum with title '{Title}' by admin {AdminId}", request.Title, request.AdminId);
            
            var validationResult = await _createValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed while creating forum: {Errors}", validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var forum = new Forum
            {
                Title = request.Title,
                Description = request.Description,
                AdminId = request.AdminId,
                CreatedDate = DateTime.UtcNow,
            };
            
            foreach (var tag in request.Tags)
            {
                var existedTag = await _tagRepository.GetByIdAsync(tag.Id);

                if (existedTag == null)
                {
                    _logger.LogWarning("Tag with id {TagId} not found when creating forum", tag.Id);
                    throw new KeyNotFoundException($"Tag with id {tag.Id} not found.");
                }
                
                forum.Tags.Add(existedTag);
            }

            await _forumRepository.AddAsync(forum);
            
            _logger.LogInformation("Forum {ForumId} created successfully", forum.Id);
        }

        public async Task UpdateForum(ForumUpdateRequest request)
        {
            _logger.LogInformation("Updating forum {ForumId}", request.Id);
            
            var validationResult = await _updateValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed while updating forum {ForumId}: {Errors}", request.Id, validationResult.Errors);
                throw new ValidationException(validationResult.Errors);
            }

            var forum = await _forumRepository.GetByIdAsync(request.Id);
            if (forum == null)
            {
                _logger.LogWarning("Forum {ForumId} not found for update", request.Id);
                throw new KeyNotFoundException($"Forum with id {request.Id} not found.");
            }
            
            forum.Title = request.Title;
            forum.Description = request.Description;
            
            forum.Tags.Clear();

            foreach (var tag in request.Tags)
            {
                var existedTag = await _tagRepository.GetByIdAsync(tag.Id);
                
                if (existedTag == null)
                {
                    _logger.LogWarning("Tag with id {TagId} not found when updating forum {ForumId}", tag.Id, request.Id);
                    throw new KeyNotFoundException($"Tag with id {tag.Id} not found.");
                }
                
                forum.Tags.Add(existedTag);
            }

            await _forumRepository.UpdateAsync(forum);
            
            _logger.LogInformation("Forum {ForumId} updated successfully", forum.Id);
        }

        public async Task DeleteForum(Guid id)
        {
            _logger.LogInformation("Deleting forum {ForumId}", id);

            var forum = await _forumRepository.GetByIdAsync(id);
            if (forum == null)
            {
                _logger.LogWarning("Forum {ForumId} not found for deletion", id);
                throw new KeyNotFoundException($"Forum with id {id} not found.");
            }

            await _forumRepository.DeleteAsync(forum);
            _logger.LogInformation("Forum {ForumId} deleted successfully", id);
        }
    }
}