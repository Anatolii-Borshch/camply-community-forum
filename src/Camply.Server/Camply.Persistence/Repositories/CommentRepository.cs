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
            var comments = await _context.Comments
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Replies)
                    .ThenInclude(r => r.User)
                .OrderByDescending(c => c.CreatedDate)
                .Take(numberOfComments)
                .ToListAsync();

            return comments;
        }

        public async Task<Comment?> GetByIdWithRepliesAsync(Guid id)
        {
            return await _context.Comments
                .Include(x => x.User)
                .Include(x => x.Replies)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}