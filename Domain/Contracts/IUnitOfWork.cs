namespace Domain.Contracts;

public interface IUnitOfWork
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<int> SaveAsync();
}
