using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;
using TouristsCore.Repositories;

namespace TouristsCore;

public interface IUnitOfWork : IAsyncDisposable
{
    IGenericRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> CompleteAsync();
    public DbContext Context { get; }
}