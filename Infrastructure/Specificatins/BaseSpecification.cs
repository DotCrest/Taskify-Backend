using System.Linq.Expressions;

namespace Infrastructure.Specificatins
{
    public abstract class BaseSpecification<TEntity>
        where TEntity : class
    {
        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
            => Criteria = criteria;
        public Expression<Func<TEntity, bool>> Criteria { get; } = default!;
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();
        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }
        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
            => IncludeExpressions.Add(includeExpression);
        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
            => OrderBy = orderByExpression;
        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescExpression)
            => OrderByDescending = orderByDescExpression;

    }
}
