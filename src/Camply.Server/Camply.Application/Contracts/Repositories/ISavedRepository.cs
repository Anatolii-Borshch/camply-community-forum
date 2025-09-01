using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface ISavedRepository : IGenericRepository<SavedPost>
    {
        Task<SavedPost?> GetByUserAndPostAsync(Guid userId, Guid postId);
    }
}