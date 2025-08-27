using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;

namespace Camply.Persistence.Repositories
{
    public class ForumRepository : GenericRepository<Forum>, IForumRepository
    {
        public ForumRepository(CamplyDbContext context) : base(context)
        {
        }
    }
}