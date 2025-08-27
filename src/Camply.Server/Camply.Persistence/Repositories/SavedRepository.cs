using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;

namespace Camply.Persistence.Repositories
{
    public class SavedRepository : GenericRepository<SavedPost>, ISavedRepository
    {
        public SavedRepository(CamplyDbContext context) : base(context)
        {
        }
    }
}