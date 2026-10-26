using CoffeeShop.Infrastructure.Models;

namespace CoffeeShop.Infrastructure.Services;

/// <summary>
/// Інтерфейс асинхронного дженерік CRUD сервісу з доступом до БД
/// </summary>
/// <typeparam name="T">Тип сутності моделі БД, що реалізує IEntityModel</typeparam>
public interface ICrudServiceDbAsync<T> : IEnumerable<T> where T : class, IEntityModel
{
    Task<bool> CreateAsync(T element);
    Task<T?> ReadAsync(int id);
    Task<IEnumerable<T>> ReadAllAsync();
    Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    Task<bool> UpdateAsync(T element);
    Task<bool> RemoveAsync(T element);
    Task<bool> SaveAsync();
}
