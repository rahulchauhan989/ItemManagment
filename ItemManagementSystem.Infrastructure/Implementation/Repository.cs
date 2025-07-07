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
        var result = await _entities.FindAsync(id);
        return result;
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
        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }

}
