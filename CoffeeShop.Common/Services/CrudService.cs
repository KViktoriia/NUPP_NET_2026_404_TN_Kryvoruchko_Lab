using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using CoffeeShop.Common.Interfaces;

namespace CoffeeShop.Common.Services;

// Дженерік CRUD сервіс для сутностей кав'ярні
public class CrudService<T> : ICrudService<T> where T : class, IEntity
{
    // Внутрішня колекція для зберігання даних
    private readonly List<T> _items = new();

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    // Створення елемента (Create)
    public void Create(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        if (_items.Any(x => x.Id == element.Id))
        {
            throw new InvalidOperationException($"Елемент із Id {element.Id} вже існує.");
        }

        _items.Add(element);
    }

    // Читання за Id (Read)
    public T Read(Guid id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return item ?? throw new KeyNotFoundException($"Елемент із Id {id} не знайдено.");
    }

    // Отримання всіх елементів (ReadAll)
    public IEnumerable<T> ReadAll()
    {
        return _items.AsReadOnly();
    }

    // Оновлення елемента (Update)
    public void Update(T element)
    {
        ArgumentNullException.ThrowIfNull(element);

        var index = _items.FindIndex(x => x.Id == element.Id);
        if (index == -1)
        {
            throw new KeyNotFoundException($"Елемент із Id {element.Id} не знайдено для оновлення.");
        }

        _items[index] = element;
    }

    // Видалення елемента (Remove)
    public void Remove(T element)
    {
        ArgumentNullException.ThrowIfNull(element);
        _items.RemoveAll(x => x.Id == element.Id);
    }

    // Додаткове завдання: Збереження даних у файл (Save)
    public void Save(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));
        }

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(_items, _jsonOptions);
        File.WriteAllText(filePath, json);
    }

    // Додаткове завдання: Завантаження даних із файлу (Load)
    public void Load(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Файл за шляхом '{filePath}' не знайдено.");
        }

        string json = File.ReadAllText(filePath);
        var loadedItems = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);

        _items.Clear();
        if (loadedItems != null)
        {
            _items.AddRange(loadedItems);
        }
    }
}
