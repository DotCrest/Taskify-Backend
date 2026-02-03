namespace Domain.Contracts;

public interface IUnitOfWork<T> where T : class
{
    Task<int> SaveAsync();
    IGenericRepository<T> Repo { get; }
}
