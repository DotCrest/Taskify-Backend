using Domain.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace Application.Specifications
{
    public abstract class BaseSpecification<TEntity> : ISpecification<TEntity>
        where TEntity : class
    {
        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
            => Criteria = criteria;
        public Expression<Func<TEntity, bool>> Criteria { get; } = default!;
        public List<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = new();
        public int Skip { get; private set; }
        public int Take { get; private set; }
        public bool IsPaginated { get; private set; }
        public List<string> SortExpressions { get; private set; } = [];

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
            => IncludeExpressions.Add(includeExpression);
        protected void ApplyPagination(int pageSize, int pageIndex)
        {
            IsPaginated = true;
            Take = pageSize;
            Skip = (pageIndex - 1) * pageSize;
        }
        protected void ApplySorting<T>(string sortBy)
        {
            // the `sortBy` parameter is expected to be like that => priority asc, createdAt desc
            var allowedProperties = typeof(T)
                                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .Select(p => p.Name)
                                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            foreach (var part in sortBy.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                // the `part` will be like that => priority asc
                var tokens = part.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0 || !allowedProperties.Contains(tokens[0]))
                    continue;
                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase) ? "descending" : "ascending";
                SortExpressions.Add($"{tokens[0]} {direction}");
            }
        }
    }
}
