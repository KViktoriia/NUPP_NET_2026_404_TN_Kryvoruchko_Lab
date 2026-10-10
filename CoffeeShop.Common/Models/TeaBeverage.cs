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

    /// <summary>
    /// Фабричний статичний метод для створення нового чайного напою із згенерованими даними
    /// </summary>
    public static new TeaBeverage CreateNew()
    {
        string[] names = ["Матча Латте", "Зелений Сенча", "Ерл Грей Преміум", "Улун Те Гуань Інь", "Трав'яний Карпатський", "Чорний Ассам", "Жасминовий Молі Хуа", "Ройбуш Ваніль"];
        string[] varieties = ["Японська Сенча", "Цейлонський Оранж Пеко", "Китайський Те Гуань Інь", "Альпійський луг", "Імбирно-обліпиховий"];
        int[] volumes = [250, 300, 350, 400, 500];

        string name = names[Random.Shared.Next(names.Length)];
        string variety = varieties[Random.Shared.Next(varieties.Length)];
        int volume = volumes[Random.Shared.Next(volumes.Length)];
        decimal price = Math.Round((decimal)(40 + Random.Shared.NextDouble() * 70), 2);
        int brewingTemp = Random.Shared.Next(70, 96);
        int steepingMinutes = Random.Shared.Next(2, 7);
        bool hasHoney = Random.Shared.Next(2) == 1;

        return new TeaBeverage(name, price, volume, variety, brewingTemp, steepingMinutes, hasHoney);
    }
}
