using Camply.Shared.Dtos.Tag;

namespace Camply.Shared.Dtos.Forum
{
    public record ForumUpdateRequest(Guid Id,string Title, string Description, Guid AdminId, List<TagDto> Tags);
}