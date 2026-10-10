using CoffeeShop.Common.Interfaces;

namespace CoffeeShop.Common.Interfaces;

/// <summary>
/// Інтерфейс асинхронного дженерік CRUD сервісу з підтримкою пагінації та ітерації
/// </summary>
/// <typeparam name="T">Тип сутності, що реалізує IEntity</typeparam>
public interface ICrudServiceAsync<T> : IEnumerable<T> where T : class, IEntity
{
    public Task<bool> CreateAsync(T element);
    public Task<T> ReadAsync(Guid id);
    public Task<IEnumerable<T>> ReadAllAsync();
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount);
    public Task<bool> UpdateAsync(T element);
    public Task<bool> RemoveAsync(T element);
    public Task<bool> SaveAsync();
}
