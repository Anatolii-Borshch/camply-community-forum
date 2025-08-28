using Camply.Application.Contracts.Repositories;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class ForumRepository : GenericRepository<Forum>, IForumRepository
    {
        public ForumRepository(CamplyDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Forum>> ListAsync(BaseSpecification<Forum> spec)
        {
            IQueryable<Forum> query = _context.Set<Forum>();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            foreach (var include in spec.Includes)
                query = query.Include(include);

            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(BaseSpecification<Forum> spec)
        {
            IQueryable<Forum> query = _context.Set<Forum>();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            return await query.CountAsync();
        }
    }
}