using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Repositories;

namespace TouristsRepository;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
   private readonly TouristsContext _context;

    public GenericRepository(TouristsContext context)
    {
        _context = context;
    }

    public async void Add(T entity) => _context.Set<T>().Add(entity);

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Delete(T entity) => _context.Set<T>().Remove(entity);
    
    public void DeleteRange(IEnumerable<T> entities) => _context.Set<T>().RemoveRange(entities);

    public async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = false,
        params Expression<Func<T, object>>[] includeExpressions)
    {
        var query = _context.Set<T>().AsQueryable();
        if(asNoTracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync();

    }
    
    public async Task<T?> GetEntityByConditionAsync(Expression<Func<T, bool>> expression, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();

        if (asNoTracking)
            query = query.AsNoTracking();
        
        query = query.Where(expression);
        
        if(includes is not null  && includes.Any())
        query = includes.Aggregate(query, (cur, next) => cur.Include(next));

        return await query.FirstOrDefaultAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> criteria = null)
    {
        var  query = _context.Set<T>().AsQueryable();
        if (criteria is not null)
            query = query.Where(criteria);
        return await query.CountAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllByConditionAsync(int?pageIndex=null,int?pageSize=null, Expression<Func<T, bool>> criteria = null, Expression<Func<T, object>> orderBy = null, bool descending = false,
        bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
    {
        var query = _context.Set<T>().AsQueryable();
        
        if (asNoTracking)
            query = query.AsNoTracking();
        
        if (criteria is not null)
            query = query.Where(criteria);
        
        if(orderBy is not null)
           query =  descending ?  query.OrderByDescending(orderBy) : query.OrderBy(orderBy); 
        
        if(includes is not null  && includes.Any())
            query = includes.Aggregate(query, (cur, next) => cur.Include(next));

        if (pageIndex.HasValue && pageSize.HasValue)
        {
            query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }
        
        return await query.ToListAsync();
    }
}