using System.Linq.Expressions;
using Camply.Domain.Entities;

namespace Camply.Application.Contracts.Repositories
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<Post?> GetIncludedByIdAsync(Guid id, params Expression<Func<Post, object>>[] includes);
    }
}