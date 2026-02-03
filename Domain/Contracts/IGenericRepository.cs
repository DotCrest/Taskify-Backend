using System.Linq.Expressions;

namespace Domain.Contracts;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> Find(Expression<Func<T, bool>> predicate);
    Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate);

}
