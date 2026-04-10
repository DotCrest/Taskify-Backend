using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Infrastructure
{
    public static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity>(IQueryable<TEntity> inputQuery,
            ISpecification<TEntity> specification) where TEntity : class
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
            if (specification.SortExpressions.Any())
            {
                query = query.OrderBy(string.Join(", ", specification.SortExpressions));
            }
            if (specification.IsPaginated)
                query = query.Skip(specification.Skip).Take(specification.Take);

            return query;
        }
    }
}
