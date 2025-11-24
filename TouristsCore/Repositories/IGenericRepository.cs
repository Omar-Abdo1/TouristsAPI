using System.Linq.Expressions;

namespace TouristsCore.Repositories;

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void DeleteRange(IEnumerable<T> entities);
    
    Task<T?> GetEntityByConditionAsync(Expression<Func<T, bool>> expression, bool asNoTracking = false,
        params Expression<Func<T, object>>[] includes);

    Task<int> CountAsync(Expression<Func<T, bool>> criteria = null); 
    
    Task<IReadOnlyList<T>> GetAllByConditionAsync(
        int? pageIndex=null ,int? pageSize=null, 
        Expression<Func<T, bool>> criteria = null,
        Expression<Func<T, object>> orderBy = null,
        bool descending = false,
        bool asNoTracking = false,
        params Expression<Func<T, object>>[] includes);
}