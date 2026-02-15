using Domain.Contracts;
using Infrastructure.context;

using Microsoft.EntityFrameworkCore;
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
        public async Task<IEnumerable<T>> GetAllAsync(ISpecification<T> specification)
        {
            return await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<T?> GetByIdAsync(ISpecification<T> specification)
        {
            return await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).FirstOrDefaultAsync();
        }
        public Task<T?> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefaultAsync(predicate);
        }
        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }
        public async Task<IEnumerable<T>> FindAll(ISpecification<T> specification)
            => await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).ToListAsync();

        public async Task<int> CountAsync(ISpecification<T> specification)
            => await SpecificationEvaluator.CreateQuery(context.Set<T>(), specification).CountAsync();
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

        public async Task BulkDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await _dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
