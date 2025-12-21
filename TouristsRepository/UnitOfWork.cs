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
    
    
    private readonly Hashtable _repositories;
    // key value pair   NameOfModel : GenericRepository<Model>  string->object
    
    private IChatRepository _chatRepository;

    public UnitOfWork(TouristsContext context)
    {
        _context = context;
        _repositories = new Hashtable();
    }
    public IChatRepository ChatRepository
    {
        get
        {
            return _chatRepository?? new ChatRepository(_context);
        }
    }

    public ValueTask DisposeAsync() => _context.DisposeAsync();

    public DbContext Context { get => _context; }

    public IGenericRepository<T> Repository<T>() where T : BaseEntity
    {
        var Type = typeof(T).Name;
        if (!_repositories.ContainsKey(Type)) // First Time
        {
            var repositoryType = typeof(GenericRepository<T>);
            var repositoryInstance = Activator.CreateInstance(repositoryType, _context);
            _repositories.Add(Type, repositoryInstance);
        }

        return _repositories[Type] as IGenericRepository<T>;
    }

    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
}