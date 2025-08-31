using Camply.Application.Contracts.Repositories;
using Camply.Application.Contracts.Services;
using Camply.Domain.Entities;
using Camply.Shared.Dtos.Tag;

namespace Camply.Application.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        
        public async Task<IEnumerable<TagDto>> GetTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();

            var mappedTags = tags.Select(x => new TagDto(x.Id, x.Name));
            
            return mappedTags;
        }

        public async Task AddTagAsync(string name)
        {
            var tags = await _tagRepository.GetAllAsync();
            
            if(tags.Any(x => x.Name.ToLower() == name.ToLower()))
                throw new ArgumentException("Tag already exists");

            var tag = new Tag()
            {
                Name = name
            };
            
            await _tagRepository.AddAsync(tag);
        }

        public async Task UpdateTagAsync(TagUpdateRequest request)
        {
            var tag = await _tagRepository.GetByIdAsync(request.Id);
            
            if(tag == null)
                throw new ArgumentException("Tag not found");
            
            var tags = await _tagRepository.GetAllAsync();
            
            if(tags.Any(x => x.Name.ToLower() == request.Name.ToLower()))
                throw new ArgumentException("Tag already exists");
            
            tag.Name = request.Name;
            
            await _tagRepository.UpdateAsync(tag);
        }

        public async Task DeleteTagAsync(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            
            if(tag == null)
                throw new ArgumentException("Tag not found");
            
            await _tagRepository.DeleteAsync(tag);
        }
    }
}