namespace Camply.Shared.Dtos.Post
{
    public record PostCreateRequest(string Title, string? Description, Guid AuthorId);
}