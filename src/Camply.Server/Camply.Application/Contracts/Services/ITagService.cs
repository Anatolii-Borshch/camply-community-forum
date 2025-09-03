using Camply.Shared.Dtos.Tag;

namespace Camply.Application.Contracts.Services
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetTagsAsync();
        Task AddTagAsync(string name, Guid userId);
        Task UpdateTagAsync(TagUpdateRequest request);
        Task DeleteTagAsync(Guid id, Guid userId);
    }
}