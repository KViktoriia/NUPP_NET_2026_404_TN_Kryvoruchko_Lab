using System.Collections;
using CoffeeShop.Infrastructure.Models;
using CoffeeShop.Infrastructure.Repositories;

namespace CoffeeShop.Infrastructure.Services;

/// <summary>
/// Асинхронний дженерік CRUD сервіс, що використовує репозиторій для доступу до бази даних.
/// </summary>
/// <typeparam name="T">Тип сутності бази даних, що реалізує IEntityModel</typeparam>
public class CrudServiceDbAsync<T> : ICrudServiceDbAsync<T> where T : class, IEntityModel
{
    private readonly IRepository<T> _repository;

    public CrudServiceDbAsync(IRepository<T> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Асинхронне створення елемента (Create)
    /// </summary>
    public async Task<bool> CreateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        try
        {
            await _repository.AddAsync(element);
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Асинхронне читання за числовим Id (Read)
    /// </summary>
    public async Task<T?> ReadAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// Асинхронне отримання всіх елементів (ReadAll)
    /// </summary>
    public async Task<IEnumerable<T>> ReadAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Асинхронне отримання сторінки елементів із пагінацією (ReadAll з page та amount)
    /// </summary>
    public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Номер сторінки повинен бути більшим або рівним 1.");
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Кількість елементів на сторінці не може бути від'ємною.");
        }

        var allItems = await _repository.GetAllAsync();
        return allItems.Skip((page - 1) * amount).Take(amount).ToList();
    }

    /// <summary>
    /// Асинхронне оновлення елемента (Update)
    /// </summary>
    public async Task<bool> UpdateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        try
        {
            await _repository.Update(element);
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Асинхронне видалення елемента (Remove)
    /// </summary>
    public async Task<bool> RemoveAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        try
        {
            await _repository.Delete(element);
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Збереження змін у БД
    /// </summary>
    public async Task<bool> SaveAsync()
    {
        try
        {
            await _repository.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Реалізація IEnumerable<T>
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        return _repository.GetAllAsync().GetAwaiter().GetResult().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
