using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.REST.Models;

/// <summary>
/// Модель для оновлення бариста через PUT-запит
/// </summary>
public class UpdateBaristaModel
{
    [Required(ErrorMessage = "Повне ім'я бариста обов'язкове")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Ім'я повинно містити від 2 до 200 символів")]
    public string FullName { get; set; } = string.Empty;

    [Range(0, 50, ErrorMessage = "Досвід роботи має бути від 0 до 50 років")]
    public int ExperienceYears { get; set; }

    [Required(ErrorMessage = "Зміна є обов'язковою")]
    [StringLength(50, ErrorMessage = "Назва зміни не повинна перевищувати 50 символів")]
    public string Shift { get; set; } = "Ранок";

    [Range(0.01, 10000.0, ErrorMessage = "Погодинна ставка повинна бути більше 0")]
    public decimal HourlyRate { get; set; }

    public bool IsCertified { get; set; }
}
