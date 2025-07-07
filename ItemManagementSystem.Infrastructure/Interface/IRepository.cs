using System.Linq.Expressions;

namespace ItemManagementSystem.Infrastructure.Interface;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task UpdateAsync(T entity);
    Task<T> AddAsync(T entity);
    Task DeleteAsync(T entity);
}
