using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Infrastructure.Repositories;

/// <summary>
/// Реалізація дженерік репозиторію, що використовує CoffeeShopContext для доступу до даних
/// </summary>
/// <typeparam name="T">Тип сутності</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CoffeeShopContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CoffeeShopContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public Task Update(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task Delete(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
