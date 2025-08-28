using Camply.Shared.Dtos.Comment;

namespace Camply.Application.Contracts.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetPostCommentsAsync(Guid postId, int numberOfComments);
        Task CreateComment (CommentCreateRequest request);
        Task UpdateComment(CommentUpdateRequest request);
        Task DeleteComment(Guid id, Guid userId);
    }
}