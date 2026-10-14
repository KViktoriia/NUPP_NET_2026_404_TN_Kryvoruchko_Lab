namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Модель напою для бази даних (базовий клас, TPT)
/// </summary>
public class BeverageModel : IEntityModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int VolumeMl { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Навігаційна властивість: багато-до-багатьох з OrderReceiptModel
    public ICollection<OrderBeverageModel> OrderBeverages { get; set; } = new List<OrderBeverageModel>();
}
