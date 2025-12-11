using System.Linq.Expressions;
using TouristsCore.Entities;

namespace TouristsCore.Repositories;

public interface IGenericRepository<T> where T : BaseEntity
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void DeletePermanently(T entity);
    void DeletePermanentlyRange(IEnumerable<T> entites);
    
    Task<T?> GetEntityByConditionAsync(Expression<Func<T, bool>> expression, bool asNoTracking = false,
        params Expression<Func<T, object>>[] includes);
    
    Task<T?>GetByIdAsync(int id,bool asNoTracking=false,params Expression<Func<T, object>>[] includes);

    Task<int> CountAsync(Expression<Func<T, bool>> criteria = null); 
    
    Task<IReadOnlyList<T>> GetAllByConditionAsync(
        int? pageIndex=null ,int? pageSize=null, 
        Expression<Func<T, bool>> criteria = null,
        Expression<Func<T, object>> orderBy = null,
        bool descending = false,
        bool asNoTracking = false,
        params Expression<Func<T, object>>[] includes);
}