using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Tag;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Camply.Application.Implementations
{
    public class TagService : UserPermissionService, ITagService
    {
        private readonly ITagRepository _tagRepository;
        
        private readonly ILogger<TagService> _logger;
        
        private readonly IMemoryCache _cache;

        private const string TagsCacheKey = "all_tags";
        public TagService(ITagRepository tagRepository, ILogger<TagService> logger
            , IUserRepository userRepository, IMemoryCache cache) 
            : base(userRepository, logger)
        {
            _tagRepository = tagRepository;
            _logger = logger;
            _cache = cache;
        }
        
        public async Task<IEnumerable<TagDto>> GetTagsAsync()
        {
            _logger.LogInformation("Fetching all tags");

            if (_cache.TryGetValue(TagsCacheKey, out IEnumerable<TagDto>? cachedTags))
            {
                _logger.LogInformation("Returned {Count} tags from cache", cachedTags.Count());
                return cachedTags;
            }
            
            var tags = await _tagRepository.GetAllAsync();
            var mappedTags = tags.Select(x => new TagDto(x.Id, x.Name));
            
            _cache.Set(TagsCacheKey, mappedTags, TimeSpan.FromMinutes(10));
            
            _logger.LogInformation("Fetched {Count} tags", mappedTags.Count());
            
            return mappedTags;
        }

        public async Task AddTagAsync(string name, Guid userId)
        {
            _logger.LogInformation("Adding new tag '{TagName}'", name);
            
            var user = await EnsureUserExistsAsync(userId);
            EnsureUserAcess(user, null);
            
            var tags = await _tagRepository.GetAllAsync();
            if (tags.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Tag '{TagName}' already exists", name);
                throw new ArgumentException("Tag already exists");
            }

            var tag = new Tag()
            {
                Name = name
            };
            
            await _tagRepository.AddAsync(tag);
            
            _cache.Remove(TagsCacheKey);
            
            _logger.LogInformation("Tag '{TagName}' added successfully with id {TagId}", tag.Name, tag.Id);
        }

        public async Task UpdateTagAsync(TagUpdateRequest request)
        {
            _logger.LogInformation("Updating tag {TagId} to new name '{TagName}'", request.Id, request.Name);
            
            var user = await EnsureUserExistsAsync(request.UserId);
            EnsureUserAcess(user, null);
            
            var tag = await _tagRepository.GetByIdAsync(request.Id);
            if (tag == null)
            {
                _logger.LogWarning("Tag {TagId} not found for update", request.Id);
                throw new ArgumentException("Tag not found");
            }
            
            var tags = await _tagRepository.GetAllAsync();
            if (tags.Any(x => x.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Tag name '{TagName}' already exists", request.Name);
                throw new ArgumentException("Tag already exists");
            }
            
            tag.Name = request.Name;
            await _tagRepository.UpdateAsync(tag);
            
            _cache.Remove(TagsCacheKey);
            
            _logger.LogInformation("Tag {TagId} updated successfully to '{TagName}'", tag.Id, tag.Name);
        }

        public async Task DeleteTagAsync(Guid id, Guid userId)
        {
            _logger.LogInformation("Deleting tag {TagId}", id);
            
            var user = await EnsureUserExistsAsync(userId);
            EnsureUserAcess(user, null);
            
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                _logger.LogWarning("Tag {TagId} not found for deletion", id);
                throw new ArgumentException("Tag not found");
            }
            
            await _tagRepository.DeleteAsync(tag);
            
            _cache.Remove(TagsCacheKey);
            
            _logger.LogInformation("Tag {TagId} deleted successfully", id);
        }
    }
}