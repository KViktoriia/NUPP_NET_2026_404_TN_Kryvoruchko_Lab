namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Проміжна таблиця для зв'язку багато-до-багатьох між OrderReceiptModel та BeverageModel
/// </summary>
public class OrderBeverageModel
{
    public int OrderReceiptId { get; set; }
    public OrderReceiptModel OrderReceipt { get; set; } = null!;

    public int BeverageId { get; set; }
    public BeverageModel Beverage { get; set; } = null!;

    // Додаткове поле для кількості
    public int Quantity { get; set; } = 1;
}
