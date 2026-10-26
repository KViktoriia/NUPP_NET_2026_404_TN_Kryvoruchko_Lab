using CoffeeShop.Common.Models;

namespace CoffeeShop.Common.Extensions;

// Статичний клас для методів розширення
public static class CoffeeShopExtensions
{
    // метод розширення
    public static string ToFormattedReceiptString(this Beverage beverage)
    {
        if (beverage is CoffeeBeverage coffee)
        {
            return $"\"{coffee.Name}\" (Походження: {coffee.CoffeeBeansOrigin}, Молоко: {coffee.MilkType})";
        }
        if (beverage is TeaBeverage tea)
        {
            return $"\"{tea.Name}\" (Сорт: {tea.TeaVariety}, Температура: {tea.BrewingTemperatureC}°C)";
        }
        return $"\"{beverage.Name}\" ({beverage.VolumeMl} мл, {beverage.Price:F2} грн)";
    }

    // метод розширення
    public static decimal CalculateAveragePrice(this IEnumerable<Beverage> beverages)
    {
        var list = beverages.ToList();
        if (!list.Any()) return 0m;
        return list.Average(b => b.Price);
    }
}
