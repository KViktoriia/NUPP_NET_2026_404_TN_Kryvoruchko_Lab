using CoffeeShop.Common.Interfaces;

namespace CoffeeShop.Common.Models;

// Клас чека замовлення
public class OrderReceipt : IEntity
{
    // Властивості
    public Guid Id { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = "Гість";
    public List<Guid> BeverageIds { get; set; } = new();

    // конструктор (за замовчуванням)
    public OrderReceipt()
    {
        Id = Guid.NewGuid();
        ReceiptNumber = $"REC-{DateTime.UtcNow.Ticks % 100000:D5}";
    }

    // конструктор (параметризований)
    public OrderReceipt(string customerName, decimal totalAmount, List<Guid> beverageIds)
    {
        Id = Guid.NewGuid();
        ReceiptNumber = $"REC-{DateTime.UtcNow.Ticks % 100000:D5}";
        CustomerName = customerName;
        TotalAmount = totalAmount;
        BeverageIds = beverageIds;
        CreatedAt = DateTime.UtcNow;
    }

    // метод
    public string GetReceiptDetails()
    {
        return $"Чек #{ReceiptNumber} для {CustomerName} на суму {TotalAmount} грн. Кількість позицій: {BeverageIds.Count}. Час: {CreatedAt:yyyy-MM-dd HH:mm:ss}";
    }
}
