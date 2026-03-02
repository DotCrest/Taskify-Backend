using System.Linq.Expressions;

namespace Domain.Contracts
{
    public interface ISpecification<TEntity> where TEntity : class
    {
        public Expression<Func<TEntity, bool>> Criteria { get; }
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        public int Skip { get; }
        public int Take { get; }
        public List<string> SortExpressions { get; }
        public bool IsPaginated { get; }
    }
}
