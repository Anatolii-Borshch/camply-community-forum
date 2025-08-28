namespace Camply.Shared.Dtos.Post
{
    public record PostUpdateRequest(Guid PostId, string Title, string? Description, Guid AuthorId);
}