using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface ILikedPostRepository : IGenericRepository<LikedPost>
    {
        Task<LikedPost?> GetByUserAndPostAsync(Guid userId, Guid postId);
    }
}