using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specificatins
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity>(IQueryable<TEntity> inputQuery,
            BaseSpecification<TEntity> specification) where TEntity : class
        {
            var query = inputQuery;
            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);
            if (specification.IncludeExpressions.Any())
            {
                query = specification.IncludeExpressions.Aggregate(query,
                (current, includeExpression)
                => current.Include(includeExpression));
            }
            if (specification.OrderBy != null)
                query = query.OrderBy(specification.OrderBy);
            if (specification.OrderByDescending != null)
                query = query.OrderByDescending(specification.OrderByDescending);
            if (specification.IsPaginated)
                query = query.Skip(specification.Skip).Take(specification.Take);
            return query;
        }
    }
}
