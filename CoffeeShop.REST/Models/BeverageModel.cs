namespace CoffeeShop.REST.Models;

/// <summary>
/// DTO модель напою для відповідей REST API
/// </summary>
public class BeverageModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int VolumeMl { get; set; }
    public bool IsAvailable { get; set; }
}
