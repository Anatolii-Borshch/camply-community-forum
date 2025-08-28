namespace Camply.Shared.Dtos.Comment
{
    public record CommentUpdateRequest(Guid Id, Guid UserId, string Content);
}