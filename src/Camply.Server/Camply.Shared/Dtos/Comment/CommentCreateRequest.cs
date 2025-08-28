namespace Camply.Shared.Dtos.Comment
{
    public record CommentCreateRequest(Guid UserId, Guid PostId, string Content, Guid? ParentCommentId);
}