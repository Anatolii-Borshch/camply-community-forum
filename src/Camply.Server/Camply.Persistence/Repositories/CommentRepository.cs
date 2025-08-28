using Camply.Application.Contracts.Repositories;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(CamplyDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(Guid postId, int numberOfComments)
        {
            return await _context.Comments
                .Include(x => x.User)
                .Include(x => x.Replies)
                .ThenInclude(x => x.User)
                .Where(x => x.PostId == postId && x.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedDate)
                .Take(numberOfComments)
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdWithRepliesAsync(Guid id)
        {
            return await _context.Comments
                .Include(x => x.UserId)
                .Include(x => x.Replies)
                .ThenInclude(x => _context.Users)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}