using System.Linq.Expressions;
using ItemManagementSystem.Domain.DataContext;
using ItemManagementSystem.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace ItemManagementSystem.Infrastructure.Implementation;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _entities;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var entity = await _entities.FindAsync(id);
        if (entity != null)
        {
            var prop = typeof(T).GetProperty("IsDeleted");
            if (prop != null && prop.GetValue(entity) is bool isDeleted && isDeleted)
                return null;
        }
        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        var param = Expression.Parameter(typeof(T), "e");
        var prop = Expression.Property(param, "IsDeleted");
        var notDeleted = Expression.Not(prop);
        var lambda = Expression.Lambda<Func<T, bool>>(notDeleted, param);

        return await _entities.Where(lambda).ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        var param = Expression.Parameter(typeof(T), "e");
        var prop = Expression.Property(param, "IsDeleted");
        var notDeleted = Expression.Not(prop);
        var softDeleteLambda = Expression.Lambda<Func<T, bool>>(notDeleted, param);

        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(predicate, param),
                Expression.Invoke(softDeleteLambda, param)
            ),
            param
        );
        return await _entities.Where(combined).ToListAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _entities.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _entities.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(T entity)
    {
        var prop = typeof(T).GetProperty("IsDeleted");
        if (prop != null && prop.PropertyType == typeof(bool))
        {
            prop.SetValue(entity, true);
            _entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }


    public async Task<IEnumerable<T>> FindIncludingAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _entities;
        foreach (var includeProperty in includeProperties)
            query = query.Include(includeProperty);

        // Soft delete support
        var param = Expression.Parameter(typeof(T), "e");
        var prop = Expression.Property(param, "IsDeleted");
        var notDeleted = Expression.Not(prop);
        var softDeleteLambda = Expression.Lambda<Func<T, bool>>(notDeleted, param);

        var combined = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(predicate, param),
                Expression.Invoke(softDeleteLambda, param)
            ),
            param
        );
        return await query.Where(combined).ToListAsync();
    }
}