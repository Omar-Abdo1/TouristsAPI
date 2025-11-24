using Microsoft.EntityFrameworkCore;
using TouristsCore.Repositories;

namespace TouristsCore;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<T> Repository<T>() where T : class;
    Task<int> CompleteAsync();
    public DbContext Context { get; }
}