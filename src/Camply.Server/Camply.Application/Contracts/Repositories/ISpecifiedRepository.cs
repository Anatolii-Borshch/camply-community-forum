using Camply.Application.Specifications;
using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface ISpecifiedRepository<T> where T : BaseEntity
    {
        Task<IReadOnlyCollection<T>> ListAsync(BaseSpecification<T> spec);
        Task<int> CountAsync(BaseSpecification<T> spec);
    }
}