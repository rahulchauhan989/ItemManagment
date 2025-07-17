using System.Linq.Expressions;
using ItemManagementSystem.Domain.Constants;
using ItemManagementSystem.Domain.DataContext;
using ItemManagementSystem.Domain.Dto.Request;
using ItemManagementSystem.Domain.Exception;
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

    public async Task<IEnumerable<T>> FindIncludingAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>>[]? includes = null)
    {
        IQueryable<T> query = _entities.AsQueryable();
        if (includes != null && includes.Length > 0)
        {
            foreach (var includeProperty in includes)
                query = query.Include(includeProperty);
        }
        return await query.Where(predicate).ToListAsync();
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

    public async Task<PagedResultDto<T>> GetPagedWithMultipleFiltersAndSortAsync(
        Dictionary<string, string?>? filterProperties,
        string? sortBy,
        string? sortDirection,
        int page,
        int pageSize)
    {
        IQueryable<T> query = _entities;

        var parameter = Expression.Parameter(typeof(T), "x");

        Expression? combinedExpression = null;

        if (filterProperties != null && filterProperties.Count > 0)
        {
            foreach (var filter in filterProperties)
            {
                if (string.IsNullOrEmpty(filter.Key) || string.IsNullOrEmpty(filter.Value))
                    continue;

                string[] propertyNames = filter.Key.Split('.');
                Expression property = parameter;
                Type propertyType = typeof(T);
                foreach (var propName in propertyNames)
                {
                    var propInfo = propertyType.GetProperty(propName);
                    if (propInfo == null)
                    {
                        throw new CustomException(AppMessages.propertyNotFound);
                    }
                    property = Expression.Property(property, propInfo); //x.name
                    propertyType = propInfo.PropertyType;
                }

                Expression filterExpression;

                if (propertyType == typeof(string))
                {
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

                    var searchTermExpression = Expression.Constant(filter.Value.ToLower());
                    var toLowerExpression = Expression.Call(property, toLowerMethod!);
                    filterExpression = Expression.Call(toLowerExpression, containsMethod!, searchTermExpression);
                }
                else
                {
                    var typedValue = Convert.ChangeType(filter.Value, propertyType);
                    var constant = Expression.Constant(typedValue, propertyType);
                    filterExpression = Expression.Equal(property, constant);
                }

                if (combinedExpression == null)
                    combinedExpression = filterExpression;
                else
                    combinedExpression = Expression.AndAlso(combinedExpression, filterExpression);
            }
        }

        var notDeletedProperty = typeof(T).GetProperty("IsDeleted");
        if (notDeletedProperty != null && notDeletedProperty.PropertyType == typeof(bool))
        {
            var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
            var notDeleted = Expression.Not(isDeletedProperty);

            if (combinedExpression == null)
                combinedExpression = notDeleted;
            else
                combinedExpression = Expression.AndAlso(combinedExpression, notDeleted);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        // Sorting
        if (!string.IsNullOrEmpty(sortBy))
        {
            var propertyInfo = typeof(T).GetProperty(sortBy);
            if (propertyInfo == null)
            {
                throw new CustomException($"'{sortBy}' is not a valid property of type '{typeof(T).Name}'.");
            }

            var property = Expression.Property(parameter, propertyInfo);
            var lambda = Expression.Lambda(property, parameter);

            string methodName = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), propertyInfo.PropertyType },
                query.Expression,
                Expression.Quote(lambda));

            query = query.Provider.CreateQuery<T>(resultExpression);
        }
        else
        {
            var nameProperty = typeof(T).GetProperty("Name");
            if (nameProperty != null)
            {
                var property = Expression.Property(parameter, "Name");
                var lambda = Expression.Lambda(property, parameter);

                string methodName = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase) ? "OrderByDescending" : "OrderBy";

                var resultExpression = Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new Type[] { typeof(T), nameProperty.PropertyType },
                    query.Expression,
                    Expression.Quote(lambda));

                query = query.Provider.CreateQuery<T>(resultExpression);
            }
        }

        var totalCount = await query.CountAsync();

        var items = await query
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
}