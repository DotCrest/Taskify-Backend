using Domain.Contracts;
using Infrastructure.context;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class GenericRepository<T>(ApplicationDbContext context) : IGenericRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
        public async Task<T?> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.Where(predicate).ToListAsync();
        }
        public async Task<T?> Find(ISpecification<T> specification)
        {
            return await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<T>> FindAll(ISpecification<T> specification)
            => await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).ToListAsync();

        public async Task<int> CountAsync(ISpecification<T> specification)
            => await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).CountAsync();
        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await _dbSet.AnyAsync(predicate);
        public async Task BulkDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await _dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);
        }

        public async Task BulkUpdateAsync(ISpecification<T> specification, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateAction, CancellationToken cancellationToken = default)
        {
            var query = SpecificationEvaluator.CreateQuery(context.Set<T>(), specification);
            await query.ExecuteUpdateAsync(updateAction, cancellationToken);
        }
    }
}
