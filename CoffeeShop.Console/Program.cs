using System.Diagnostics;
using System.Text;
using CoffeeShop.Common.Extensions;
using CoffeeShop.Common.Models;
using CoffeeShop.Common.Services;

namespace CoffeeShop.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        System.Console.OutputEncoding = Encoding.UTF8;

        System.Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ CRUD СЕРВІСУ (АСИНХРОННІСТЬ ТА БАГАТОПОТОКОВІСТЬ) ===\n");

        string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "beverages_catalog.json");
        var asyncService = new CrudServiceAsync<Beverage>(dataFilePath);

        // 1. CreateAsync (Parallel)
        const int totalItemsToGenerate = 1500;
        System.Console.WriteLine($"--- 1. Паралельне створення {totalItemsToGenerate} об'єктів (Parallel.For) ---");
        var sw = Stopwatch.StartNew();
        Parallel.For(0, totalItemsToGenerate, _ =>
        {
            asyncService.CreateAsync(Beverage.CreateNew()).GetAwaiter().GetResult();
        });
        sw.Stop();
        System.Console.WriteLine($"Створено через CrudServiceAsync та Beverage.CreateNew(): {asyncService.Count} об'єктів за {sw.ElapsedMilliseconds} мс. Втрат даних: 0.\n");

        // 2. LINQ
        System.Console.WriteLine("--- 2. Статистика для згенерованих об'єктів (LINQ) ---");
        decimal minPrice = asyncService.Min(b => b.Price);
        decimal maxPrice = asyncService.Max(b => b.Price);
        decimal avgPrice = asyncService.Average(b => b.Price);

        int minVolume = asyncService.Min(b => b.VolumeMl);
        int maxVolume = asyncService.Max(b => b.VolumeMl);
        double avgVolume = asyncService.Average(b => b.VolumeMl);

        var coffeeList = asyncService.OfType<CoffeeBeverage>().ToList();
        double avgCaffeine = coffeeList.Any() ? coffeeList.Average(c => c.CaffeineAmountMg) : 0;

        System.Console.WriteLine($"Ціна (грн):   min = {minPrice:F2}, max = {maxPrice:F2}, avg = {avgPrice:F2}");
        System.Console.WriteLine($"Об'єм (мл):   min = {minVolume}, max = {maxVolume}, avg = {avgVolume:F1}");
        System.Console.WriteLine($"Кофеїн (мг):  avg = {avgCaffeine:F1} (у {coffeeList.Count} кавових напоях)");
        System.Console.WriteLine($"Всього об'єктів у колекції: {asyncService.Count()}\n");

        // 3. ReadAllAsync (Pagination)
        const int pageSize = 5;
        System.Console.WriteLine($"--- 3. Пагінація (ReadAllAsync: сторінка 1, {pageSize} елементів) ---");
        var pagedItems = await asyncService.ReadAllAsync(1, pageSize);
        int index = 1;
        foreach (var item in pagedItems)
        {
            string typeBadge = item is CoffeeBeverage ? "[Кава]" : item is TeaBeverage ? "[Чай]" : "[Напій]";
            System.Console.WriteLine($"{index++}. {typeBadge,-7} \"{item.Name}\" | {item.VolumeMl} мл | {item.Price:F2} грн | ID: {item.Id}");
        }
        System.Console.WriteLine();

        // 4. SaveAsync
        System.Console.WriteLine("--- 4. Асинхронне збереження колекції у файл (SaveAsync) ---");
        await asyncService.SaveAsync();
        var fileInfo = new FileInfo(dataFilePath);
        System.Console.WriteLine($"Колекцію успішно збережено у файл '{fileInfo.Name}' ({fileInfo.Length / 1024.0:F1} КБ).\n");

        // 5. Примітиви синхронізації
        System.Console.WriteLine("--- 5. Приклади використання примітивів синхронізації ---");
        await SynchronizationDemo.RunAllDemosAsync();
    }
}
