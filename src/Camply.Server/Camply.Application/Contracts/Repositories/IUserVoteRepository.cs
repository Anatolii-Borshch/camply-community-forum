using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface IUserVoteRepository : IGenericRepository<UserVote>
    {
        Task<UserVote?> GetByVoteAndUserAsync(Guid voteId, Guid userId);
    }
}