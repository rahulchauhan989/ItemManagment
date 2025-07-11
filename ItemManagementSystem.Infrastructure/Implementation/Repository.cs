using System.Linq;
using System.Linq.Expressions;
using ItemManagementSystem.Domain.DataContext;
using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto.Request;
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
        return entity;
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _entities.ToListAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _entities.Where(predicate).ToListAsync();
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

    public async Task<PagedResultDto<T>> GetPagedAsync(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
    int page,
    int pageSize)
    {
        var query = _entities.Where(filter);

        var totalCount = await query.CountAsync();
        var items = await orderBy(query)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // public async Task<PagedResultDto<ItemRequest>> GetPagedAsync(
    // Expression<Func<ItemRequest, bool>> filter,
    // Func<IQueryable<ItemRequest>, IOrderedQueryable<ItemRequest>> orderBy,
    // int page,
    // int pageSize)
    // {
    //     IQueryable<ItemRequest> query = _context.ItemRequests
    //         .Where(filter)
    //         .Include(r => r.RequestItems)
    //             .ThenInclude(ri => ri.ItemModel)
    //                 .ThenInclude(im => im.ItemType)
    //         .Include(r => r.User);

    //     var totalCount = await query.CountAsync();
    //     var items = await orderBy(query)
    //         .Skip((page - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync();

    //     return new PagedResultDto<ItemRequest>
    //     {
    //         Items = items,
    //         TotalCount = totalCount,
    //         Page = page,
    //         PageSize = pageSize
    //     };
    // }




    public async Task<PagedResultDto<T>> GetPagedAsyncWithIncludes(
    Expression<Func<T, bool>> filter,
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
    int page,
    int pageSize,
    params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _entities.Where(filter);
        foreach (var includeProperty in includeProperties)
            query = query.Include(includeProperty);

        var totalCount = await query.CountAsync();
        var items = await orderBy(query)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // public async Task<PagedResultDto<ItemRequest>> GetPagedRequestsWithItemsAsync(
    //     Expression<Func<ItemRequest, bool>> filter,
    //     Func<IQueryable<ItemRequest>, IOrderedQueryable<ItemRequest>> orderBy,
    //     int page,
    //     int pageSize)
    // {
    //     IQueryable<ItemRequest> query = _entities
    //         .Where(filter)
    //         .Include(r => r.RequestItems)
    //             .ThenInclude(ri => ri.ItemModel)
    //                 .ThenInclude(im => im.ItemType);

    //     var totalCount = await query.CountAsync();
    //     var items = await orderBy(query)
    //         .Skip((page - 1) * pageSize)
    //         .Take(pageSize)
    //         .ToListAsync();

    //     return new PagedResultDto<ItemRequest>
    //     {
    //         Items = items,
    //         TotalCount = totalCount,
    //         Page = page,
    //         PageSize = pageSize
    //     };
    // }


}