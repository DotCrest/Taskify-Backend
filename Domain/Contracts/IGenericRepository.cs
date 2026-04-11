using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Domain.Contracts;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> Find(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T?> Find(ISpecification<T> specification);
    Task<IEnumerable<T>> FindAll(ISpecification<T> specification);

    Task<int> CountAsync(ISpecification<T> specification);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task BulkUpdateAsync(ISpecification<T> specification, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> updateAction, CancellationToken cancellationToken = default);
    Task BulkDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

}
