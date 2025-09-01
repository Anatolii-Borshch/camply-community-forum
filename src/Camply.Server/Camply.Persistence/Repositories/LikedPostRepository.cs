using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class LikedPostRepository : GenericRepository<LikedPost>, ILikedPostRepository
    {
        public LikedPostRepository(CamplyDbContext context) : base(context)
        {
        }

        public async Task<LikedPost?> GetByUserAndPostAsync(Guid userId, Guid postId)
        {
            return await _context.LikedPosts.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
        }
    }
}