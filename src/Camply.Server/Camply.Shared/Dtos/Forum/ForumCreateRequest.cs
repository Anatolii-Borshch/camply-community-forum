using Camply.Shared.Dtos.Tag;

namespace Camply.Shared.Dtos.Forum
{
    public record ForumCreateRequest(string Title, string? Description, Guid AdminId, List<TagDto> Tags);
}