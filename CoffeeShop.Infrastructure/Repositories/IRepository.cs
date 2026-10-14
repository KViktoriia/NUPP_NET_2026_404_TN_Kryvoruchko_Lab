namespace CoffeeShop.Infrastructure.Repositories;

/// <summary>
/// Інтерфейс дженерік репозиторію для доступу до даних
/// </summary>
/// <typeparam name="T">Тип сутності</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task Update(T entity);
    Task Delete(T entity);
    Task SaveChangesAsync();
}
