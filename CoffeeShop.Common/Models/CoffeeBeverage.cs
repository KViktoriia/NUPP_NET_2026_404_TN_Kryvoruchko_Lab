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
}
