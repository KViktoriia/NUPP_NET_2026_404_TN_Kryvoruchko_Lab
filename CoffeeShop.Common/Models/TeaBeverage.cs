namespace CoffeeShop.Common.Models;

// Клас чайного напою (успадковується від Beverage)
public class TeaBeverage : Beverage
{
    // Властивості
    public string TeaVariety { get; set; } = "Зелений Сентя";
    public int BrewingTemperatureC { get; set; } = 80;
    public int SteepingTimeMinutes { get; set; } = 3;
    public bool HasHoney { get; set; } = false;

    // конструктор (за замовчуванням)
    public TeaBeverage() : base()
    {
    }

    // конструктор (параметризований)
    public TeaBeverage(string name, decimal price, int volumeMl, string teaVariety, int brewingTempC, int steepingMinutes, bool hasHoney)
        : base(name, price, volumeMl)
    {
        TeaVariety = teaVariety;
        BrewingTemperatureC = brewingTempC;
        SteepingTimeMinutes = steepingMinutes;
        HasHoney = hasHoney;
    }

    // метод
    public override void Prepare()
    {
        System.Console.WriteLine($"[Tea] Заварювання чаю '{TeaVariety}' при температурі {BrewingTemperatureC}°C протягом {SteepingTimeMinutes} хв. Мед: {(HasHoney ? "Так" : "Ні")}.");
    }
}
