using System.Linq.Expressions;

namespace Camply.Application.Specifications
{
    public abstract class BaseSpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }

        public int? Skip { get; private set; }
        public int? Take { get; private set; }
        
        protected BaseSpecification(Expression<Func<T, bool>>? criteria = null)
        {
            Criteria = criteria;
        }

        protected void AddInclude(Expression<Func<T, object>> includeExpression) 
            => Includes.Add(includeExpression);

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) 
            => OrderBy = orderByExpression;

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) 
            => OrderByDescending = orderByDescendingExpression;
        
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
        }
    }

}