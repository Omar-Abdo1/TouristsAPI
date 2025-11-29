using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Repositories;

namespace TouristsRepository;

public class UnitOfWork : IUnitOfWork
{
    private TouristsContext _context;
    private readonly IServiceProvider _serviceProvider;

    private readonly Hashtable _repositories;
    // key value pair   NameOfModel : GenericRepository<Model>  string->object

    public UnitOfWork(TouristsContext context,IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
        _repositories = new Hashtable();
    }

    public ValueTask DisposeAsync() => _context.DisposeAsync();

    public DbContext Context { get => _context; }

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var Type = typeof(T).Name;
        if (!_repositories.ContainsKey(Type)) // First Time
        {
            var Repository = _serviceProvider.GetRequiredService<IGenericRepository<T>>();
            _repositories.Add(Type, Repository);
        }

        return _repositories[Type] as IGenericRepository<T>;
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
}