namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Модель кавового напою для бази даних (TPT, наслідує BeverageModel)
/// </summary>
public class CoffeeBeverageModel : BeverageModel
{
    public string CoffeeBeansOrigin { get; set; } = "Бразилія";
    public string MilkType { get; set; } = "Цілісне коров'яче";
    public int CaffeineAmountMg { get; set; } = 80;
    public bool HasSyrup { get; set; } = false;
}
