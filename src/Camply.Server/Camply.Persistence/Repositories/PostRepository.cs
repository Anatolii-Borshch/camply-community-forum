using System.Linq.Expressions;
using Camply.Application.Contracts.Repositories;
using Camply.Application.Implementations;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class PostRepository : GenericRepository<Post>, IPostRepository, ISpecifiedRepository<Post>
    {
        public PostRepository(CamplyDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Post>> ListAsync(BaseSpecification<Post> spec)
        {
            IQueryable<Post> query = _context.Set<Post>();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            foreach (var include in spec.Includes)
                query = query.Include(include);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.Skip.HasValue)
                query = query.Skip(spec.Skip.Value);

            if (spec.Take.HasValue)
                query = query.Take(spec.Take.Value);

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(BaseSpecification<Post> spec)
        {
            IQueryable<Post> query = _context.Set<Post>();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            return await query.CountAsync();
        }

        public async Task<Post?> GetIncludedByIdAsync(Guid id, params Expression<Func<Post, object>>[] includes)
        {
            IQueryable<Post> query = _context.Set<Post>();

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}