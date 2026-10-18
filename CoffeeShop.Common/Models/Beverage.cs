using System.Text.Json.Serialization;
using CoffeeShop.Common.Interfaces;

namespace CoffeeShop.Common.Models;

// Базовий клас напою з підтримкою поліморфної серіалізації
[JsonDerivedType(typeof(Beverage), typeDiscriminator: "beverage")]
[JsonDerivedType(typeof(CoffeeBeverage), typeDiscriminator: "coffee")]
[JsonDerivedType(typeof(TeaBeverage), typeDiscriminator: "tea")]
public class Beverage : IEntity
{
    // Властивості
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int VolumeMl { get; set; }
    public bool IsAvailable { get; set; } = true;

    // статичне поле
    private static int _totalBeveragesCreated;

    // статичний конструктор
    static Beverage()
    {
        _totalBeveragesCreated = 0;
    }

    // делегат
    public delegate void PriceChangedHandler(Beverage beverage, decimal oldPrice, decimal newPrice);

    // подія
    public event PriceChangedHandler? OnPriceChanged;

    // конструктор (за замовчуванням)
    public Beverage()
    {
        Id = Guid.NewGuid();
        Interlocked.Increment(ref _totalBeveragesCreated);
    }

    // конструктор (параметризований)
    public Beverage(string name, decimal price, int volumeMl)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        VolumeMl = volumeMl;
        IsAvailable = true;
        Interlocked.Increment(ref _totalBeveragesCreated);
    }

    // метод
    public virtual void Prepare()
    {
        System.Console.WriteLine($"[Beverage] Приготування базового напою: {Name} ({VolumeMl} мл).");
    }

    // метод
    public void ChangePrice(decimal newPrice)
    {
        decimal oldPrice = Price;
        Price = newPrice;
        // Виклик події
        OnPriceChanged?.Invoke(this, oldPrice, newPrice);
    }

    // статичний метод
    public static int GetTotalBeveragesCreated()
    {
        return _totalBeveragesCreated;
    }

    /// <summary>
    /// Фабричний статичний метод для створення нового напою з випадковими даними
    /// </summary>
    public static Beverage CreateNew()
    {
        int choice = Random.Shared.Next(3);
        return choice switch
        {
            0 => CoffeeBeverage.CreateNew(),
            1 => TeaBeverage.CreateNew(),
            _ => CreateNewBaseBeverage()
        };
    }

    /// <summary>
    /// Фабричний статичний метод для створення базового безалкогольного напою
    /// </summary>
    public static Beverage CreateNewBaseBeverage()
    {
        string[] names = ["Лимонад цитрусовий", "Смузі ягідний", "Какао з маршмелоу", "Гарячий шоколад", "Фреш апельсиновий", "Мохіто б/а"];
        int[] volumes = [200, 250, 300, 350, 400, 500];

        string name = names[Random.Shared.Next(names.Length)];
        decimal price = Math.Round((decimal)(45 + Random.Shared.NextDouble() * 80), 2);
        int volume = volumes[Random.Shared.Next(volumes.Length)];

        return new Beverage(name, price, volume);
    }
}
