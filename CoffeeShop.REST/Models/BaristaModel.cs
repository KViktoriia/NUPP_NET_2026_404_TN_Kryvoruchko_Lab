namespace CoffeeShop.REST.Models;

/// <summary>
/// DTO модель бариста для відповідей REST API
/// </summary>
public class BaristaModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Shift { get; set; } = "Ранок";
    public decimal HourlyRate { get; set; }
    public bool IsCertified { get; set; }
}
