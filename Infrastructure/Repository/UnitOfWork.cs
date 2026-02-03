using Domain.Contracts;
using Infrastructure.context;

namespace Infrastructure.Repository;

public class UnitOfWork<T>(ApplicationDbContext context) : IUnitOfWork<T> where T : class
{
    private IGenericRepository<T>? repo;
    public IGenericRepository<T> Repo => repo ?? (repo = new GenericRepository<T>(context));

    public Task<int> SaveAsync()
    {
        return context.SaveChangesAsync();
    }
}
