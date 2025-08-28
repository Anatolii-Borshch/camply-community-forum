using Camply.Shared.Dtos;
using Camply.Shared.Dtos.Forum;
using Camply.Shared.Dtos.Tag;

namespace Camply.Application.Contracts.Services
{
    public interface IForumService
    {
        Task<ForumDto> GetForumById(Guid id);
        Task<IEnumerable<ForumPrevievDto>> GetAllForums(ForumSearchRequest request);
        Task CreateForum(ForumCreateRequest request);
        Task UpdateForum(ForumUpdateRequest request);
        Task DeleteForum(Guid id);
    }
}