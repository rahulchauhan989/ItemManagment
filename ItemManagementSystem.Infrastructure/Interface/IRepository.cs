using System.Linq.Expressions;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Infrastructure.Interface;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task UpdateAsync(T entity);
    Task<T> AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> FindIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
  
    Task<PagedResultDto<T>> GetPagedAsync(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
    int page,
    int pageSize);

    Task<PagedResultDto<T>> GetPagedWithMultipleFiltersAndSortAsync(
        Dictionary<string, string?>? filterProperties,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize);

    Task<PagedResultDto<T>> GetPagedAsyncWithIncludes(
        Expression<Func<T, bool>>? filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
        int page,
        int pageSize,
        params Expression<Func<T, object>>[] includeProperties);
}
