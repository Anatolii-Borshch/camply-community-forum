using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class UserVoteRepository : GenericRepository<UserVote>, IUserVoteRepository
    {
        public UserVoteRepository(CamplyDbContext context) : base(context)
        {
        }
        
        public async Task<UserVote?> GetByVoteAndUserAsync(Guid voteId, Guid userId)
        {
            return await _context.Set<UserVote>()
                .Include(x => x.Option)
                .ThenInclude(x => x.Vote)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Option.VoteId == voteId);
        }
    }
}