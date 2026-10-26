namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Модель чека замовлення для бази даних
/// </summary>
public class OrderReceiptModel : IEntityModel
{
    public int Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = "Гість";

    // Зв'язок один-до-одного: замовлення має одного основного баристу
    // (Foreign Key)
    public int BaristaId { get; set; }
    // Навігаційна властивість
    public BaristaModel Barista { get; set; } = null!;

    // Навігаційна властивість: багато-до-багатьох з BeverageModel
    public ICollection<OrderBeverageModel> OrderBeverages { get; set; } = new List<OrderBeverageModel>();
}
