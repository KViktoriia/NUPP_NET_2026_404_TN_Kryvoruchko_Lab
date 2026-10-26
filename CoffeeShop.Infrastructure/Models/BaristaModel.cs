namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Модель баристи для бази даних
/// </summary>
public class BaristaModel : IEntityModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Shift { get; set; } = "Ранкова";
    public decimal HourlyRate { get; set; }
    public bool IsCertified { get; set; }

    // Навігаційна властивість: один-до-багатьох (один бариста -> багато замовлень)
    public ICollection<OrderReceiptModel> Orders { get; set; } = new List<OrderReceiptModel>();
}
