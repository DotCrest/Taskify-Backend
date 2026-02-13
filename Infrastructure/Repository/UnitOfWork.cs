using Domain.Contracts;
using Infrastructure.context;

namespace Infrastructure.Repository;

public class UnitOfWork<T>(ApplicationDbContext context) : IUnitOfWork<T> where T : class
{
    private IGenericRepository<T>? repo;
    public IGenericRepository<T> Repo => repo ?? (repo = new GenericRepository<T>(context));

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
