namespace Domain.Contracts;

public interface IUnitOfWork<T> where T : class
{
    Task ExecuteInTransactionAsync(Func<Task> action);
    Task<int> SaveAsync();
    IGenericRepository<T> Repo { get; }
}
