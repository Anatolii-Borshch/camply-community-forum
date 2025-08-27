using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;

namespace Camply.Persistence.Repositories
{
    public class UserVoteRepository : GenericRepository<UserVote>, IUserVoteRepository
    {
        public UserVoteRepository(CamplyDbContext context) : base(context)
        {
        }
    }
}