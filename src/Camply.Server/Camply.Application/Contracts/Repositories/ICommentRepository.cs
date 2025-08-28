using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId, int numberOfComments);
        Task<Comment?> GetByIdWithRepliesAsync(Guid id);
    }
}