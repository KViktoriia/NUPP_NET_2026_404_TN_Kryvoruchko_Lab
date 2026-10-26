namespace CoffeeShop.Common.Models;

// Клас кавового напою (успадковується від Beverage)
public class CoffeeBeverage : Beverage
{
    // Властивості
    public string CoffeeBeansOrigin { get; set; } = "Бразилія";
    public string MilkType { get; set; } = "Цілісне коров'яче";
    public int CaffeineAmountMg { get; set; } = 80;
    public bool HasSyrup { get; set; } = false;

    // конструктор (за замовчуванням)
    public CoffeeBeverage() : base()
    {
    }

    // конструктор (параметризований)
    public CoffeeBeverage(string name, decimal price, int volumeMl, string beansOrigin, string milkType, int caffeineMg, bool hasSyrup)
        : base(name, price, volumeMl)
    {
        CoffeeBeansOrigin = beansOrigin;
        MilkType = milkType;
        CaffeineAmountMg = caffeineMg;
        HasSyrup = hasSyrup;
    }

    // метод
    public override void Prepare()
    {
        System.Console.WriteLine($"[Coffee] Екстракція еспресо з зерен ({CoffeeBeansOrigin}), додавання молока ({MilkType}), вміст кофеїну {CaffeineAmountMg} мг.");
    }

    // метод
    public void AddSyrup(string syrupFlavor)
    {
        HasSyrup = true;
        System.Console.WriteLine($"[Coffee] Додано сироп зі смаком '{syrupFlavor}' до напою {Name}.");
    }

    /// <summary>
    /// Фабричний статичний метод для створення нового кавового напою із згенерованими даними
    /// </summary>
    public static new CoffeeBeverage CreateNew()
    {
        string[] names = ["Еспресо Допіо", "Капучино", "Лате Макіато", "Флет Вайт", "Раф Ванільний", "Американо", "Фільтр Кенія", "Кортадо", "Мокачино"];
        string[] origins = ["Ефіопія Іргачіф", "Колумбія Супремо", "Бразилія Сантос", "Гватемала Антигуа", "Кенія АА", "Коста-Ріка Тарразу"];
        string[] milks = ["Цілісне коров'яче", "Безлактозне", "Вівсяне Barista", "Мигдальне", "Кокосове", "Без молока"];
        int[] volumes = [60, 150, 200, 250, 300, 350, 400];

        string name = names[Random.Shared.Next(names.Length)];
        string origin = origins[Random.Shared.Next(origins.Length)];
        string milk = milks[Random.Shared.Next(milks.Length)];
        int volume = volumes[Random.Shared.Next(volumes.Length)];
        decimal price = Math.Round((decimal)(50 + Random.Shared.NextDouble() * 95), 2);
        int caffeine = Random.Shared.Next(40, 210);
        bool hasSyrup = Random.Shared.Next(2) == 1;

        return new CoffeeBeverage(name, price, volume, origin, milk, caffeine, hasSyrup);
    }
}
