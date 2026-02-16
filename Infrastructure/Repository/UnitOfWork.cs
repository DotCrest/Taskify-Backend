using Domain.Contracts;
using Infrastructure.context;
using System.Collections;

namespace Infrastructure.Repository;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly Hashtable _repositories = new();
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        var type = typeof(TEntity).Name;
        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), context);
            _repositories.Add(type, repositoryInstance);
        }
        return (IGenericRepository<TEntity>)_repositories[type]!;
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action)
    {
        using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task<int> SaveAsync()
    {
        return context.SaveChangesAsync();
    }
}
