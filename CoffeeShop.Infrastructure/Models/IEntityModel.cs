namespace CoffeeShop.Infrastructure.Models;

/// <summary>
/// Інтерфейс для моделей БД з числовим первинним ключем
/// </summary>
public interface IEntityModel
{
    int Id { get; set; }
}
