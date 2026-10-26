namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Модель чайного напою для бази даних (TPT, наслідує BeverageModel)
/// </summary>
public class TeaBeverageModel : BeverageModel
{
    public string TeaVariety { get; set; } = "Зелений Сентя";
    public int BrewingTemperatureC { get; set; } = 80;
    public int SteepingTimeMinutes { get; set; } = 3;
    public bool HasHoney { get; set; } = false;
}
