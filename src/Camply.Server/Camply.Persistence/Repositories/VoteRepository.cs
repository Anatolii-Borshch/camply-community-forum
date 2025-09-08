using Camply.Application.Contracts.Repositories;
using Camply.Application.Specifications;
using Camply.Domain.Entities;
using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Camply.Persistence.Repositories
{
    public class VoteRepository : GenericRepository<Vote>, IVoteRepository, ISpecifiedRepository<Vote>
    {
        public VoteRepository(CamplyDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyCollection<Vote>> ListAsync(BaseSpecification<Vote> spec)
        {
            IQueryable<Vote> query = _context.Set<Vote>();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            foreach (var include in spec.Includes)
                query = query.Include(include);

            query.Include(x => x.VoteOptions)
                .ThenInclude(x => x.UserVotes);
            
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

        public async Task<int> CountAsync(BaseSpecification<Vote> spec)
        {
            IQueryable<Vote> query = _context.Set<Vote>();

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            return await query.CountAsync();
        }
    }
}