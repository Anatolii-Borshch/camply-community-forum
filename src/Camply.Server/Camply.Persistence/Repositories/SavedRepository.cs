using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class SavedRepository : GenericRepository<SavedPost>, ISavedRepository
    {
        public SavedRepository(CamplyDbContext context) : base(context)
        {
        }

        public async Task<SavedPost?> GetByUserAndPostAsync(Guid userId, Guid postId)
        {
            return await _context.SavedPosts.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
        }
    }
}