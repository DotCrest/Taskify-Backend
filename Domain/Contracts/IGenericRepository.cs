using System.Linq.Expressions;

namespace Domain.Contracts;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetAllAsync(ISpecification<T> specification);
    Task<T?> GetByIdAsync(int id);
    Task<T?> GetByIdAsync(ISpecification<T> specification);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task BulkDeleteAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> Find(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAll(ISpecification<T> specification);

    Task<int> CountAsync(ISpecification<T> specification);

}
