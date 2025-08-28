using Camply.Application.Specifications;
using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface IForumRepository : IGenericRepository<Forum>
    {
        Task<IReadOnlyCollection<Forum>> ListAsync(BaseSpecification<Forum> spec);
        Task<int> CountAsync(BaseSpecification<Forum> spec);
    }
}