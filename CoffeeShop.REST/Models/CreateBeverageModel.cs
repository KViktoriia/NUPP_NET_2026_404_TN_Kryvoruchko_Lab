using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.REST.Models;

/// <summary>
/// Модель для створення нового напою через POST-запит
/// </summary>
public class CreateBeverageModel
{
    [Required(ErrorMessage = "Назва напою обов'язкова")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Назва напою має містити від 2 до 200 символів")]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 10000.0, ErrorMessage = "Ціна напою має бути більшою за 0")]
    public decimal Price { get; set; }

    [Range(10, 5000, ErrorMessage = "Об'єм напою має бути від 10 до 5000 мл")]
    public int VolumeMl { get; set; }

    public bool IsAvailable { get; set; } = true;
}
