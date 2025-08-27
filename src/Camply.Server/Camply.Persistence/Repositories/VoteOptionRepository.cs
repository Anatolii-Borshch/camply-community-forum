using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;

namespace Camply.Persistence.Repositories
{
    public class VoteOptionRepository : GenericRepository<VoteOption>, IVoteOptionRepository
    {
        public VoteOptionRepository(CamplyDbContext context) : base(context)
        {
        }
    }
}