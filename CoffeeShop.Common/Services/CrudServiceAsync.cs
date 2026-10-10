using System.Collections;
using System.Collections.Concurrent;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using CoffeeShop.Common.Interfaces;

namespace CoffeeShop.Common.Services;

/// <summary>
/// Асинхронний дженерік CRUD сервіс, потокобезпечний (thread-safe),
/// з підтримкою пагінації, серіалізації у файл та інтерфейсу IEnumerable.
/// </summary>
/// <typeparam name="T">Тип сутності, що реалізує IEntity</typeparam>
public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class, IEntity
{
    // Потокобезпечна вбудована колекція .NET для збереження сутностей за їхнім Id
    private readonly ConcurrentDictionary<Guid, T> _items = new();

    // Семафор для запобігання гонкам даних при паралельному записі у файл
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    /// <summary>
    /// Шлях до файлу для серіалізації/десеріалізації колекції
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Кількість елементів у сервісі на поточний момент
    /// </summary>
    public int Count => _items.Count;

    public CrudServiceAsync(string? filePath = null)
    {
        FilePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{typeof(T).Name.ToLower()}s_store.json");
    }

    /// <summary>
    /// Асинхронне додавання елемента (Create)
    /// </summary>
    public Task<bool> CreateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        // TryAdd є атомарною та потокобезпечною операцією
        bool added = _items.TryAdd(element.Id, element);
        return Task.FromResult(added);
    }

    /// <summary>
    /// Асинхронне читання за ідентифікатором (Read)
    /// </summary>
    public Task<T> ReadAsync(Guid id)
    {
        if (_items.TryGetValue(id, out var element))
        {
            return Task.FromResult(element);
        }

        return Task.FromException<T>(new KeyNotFoundException($"Елемент із Id {id} не знайдено у сервісі {typeof(T).Name}."));
    }

    /// <summary>
    /// Асинхронне отримання всіх елементів (ReadAll)
    /// </summary>
    public Task<IEnumerable<T>> ReadAllAsync()
    {
        // Повертаємо моментальний знімок (snapshot) поточної колекції
        IEnumerable<T> snapshot = _items.Values.ToList();
        return Task.FromResult(snapshot);
    }

    /// <summary>
    /// Асинхронне отримання сторінки елементів із пагінацією (ReadAll з page та amount)
    /// </summary>
    /// <param name="page">Номер сторінки (нумерація з 1)</param>
    /// <param name="amount">Кількість елементів на сторінці</param>
    public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Номер сторінки повинен бути більшим або рівним 1.");
        }

        if (amount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Кількість елементів на сторінці не може бути від'ємною.");
        }

        IEnumerable<T> pagedItems = _items.Values
            .Skip((page - 1) * amount)
            .Take(amount)
            .ToList();

        return Task.FromResult(pagedItems);
    }

    /// <summary>
    /// Асинхронне оновлення елемента (Update)
    /// </summary>
    public Task<bool> UpdateAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (!_items.ContainsKey(element.Id))
        {
            return Task.FromResult(false);
        }

        // Оновлюємо значення в словнику
        _items[element.Id] = element;
        return Task.FromResult(true);
    }

    /// <summary>
    /// Асинхронне видалення елемента (Remove)
    /// </summary>
    public Task<bool> RemoveAsync(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        bool removed = _items.TryRemove(element.Id, out _);
        return Task.FromResult(removed);
    }

    /// <summary>
    /// Асинхронне збереження колекції у серіалізованому вигляді у файлі за шляхом FilePath (Save)
    /// </summary>
    public async Task<bool> SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            throw new InvalidOperationException("Шлях до файлу (FilePath) не задано або він порожній.");
        }

        await _fileSemaphore.WaitAsync();
        try
        {
            var directory = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Робимо потокобезпечний знімок елементів для серіалізації
            var snapshot = _items.Values.ToList();

            await using var stream = new FileStream(
                FilePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize: 4096,
                useAsync: true);

            await JsonSerializer.SerializeAsync(stream, snapshot, _jsonOptions);
            return true;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    /// <summary>
    /// Асинхронне завантаження колекції з файлу за вказаним шляхом або FilePath
    /// </summary>
    public async Task<bool> LoadAsync(string? customPath = null)
    {
        string path = customPath ?? FilePath;

        if (string.IsNullOrWhiteSpace(path))
        {
            throw new InvalidOperationException("Шлях до файлу не задано.");
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Файл за шляхом '{path}' не знайдено.");
        }

        await _fileSemaphore.WaitAsync();
        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize: 4096,
                useAsync: true);

            var loadedItems = await JsonSerializer.DeserializeAsync<List<T>>(stream, _jsonOptions);

            _items.Clear();
            if (loadedItems != null)
            {
                foreach (var item in loadedItems)
                {
                    _items.TryAdd(item.Id, item);
                }
            }

            return true;
        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    /// <summary>
    /// Очищення внутрішньої колекції
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }

    /// <summary>
    /// Реалізація IEnumerable<T> для підтримки LINQ-запитів та foreach безпосередньо по сервісу
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        return _items.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
