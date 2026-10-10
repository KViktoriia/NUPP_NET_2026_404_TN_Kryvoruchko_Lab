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

        // Шлях до файлу для збереження серіалізованої колекції
        string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "beverages_catalog.json");
        var asyncService = new CrudServiceAsync<Beverage>(dataFilePath);

        // -------------------------------------------------------------------------
        // ЧАСТИНА 1. Базові операції асинхронного CRUD сервісу
        // -------------------------------------------------------------------------
        System.Console.WriteLine("--- ЧАСТИНА 1. ПЕРЕВІРКА БАЗОВИХ ОПЕРАЦІЙ ICrudServiceAsync<T> ---");
        var sampleCoffee = new CoffeeBeverage("Авторський Раф Лаванда", 110.00m, 350, "Ефіопія", "Вівсяне Barista", 95, true);
        var sampleTea = new TeaBeverage("Матча Церемоніальна", 125.00m, 300, "Японська Удзі", 75, 3, false);

        // 1. CreateAsync
        await asyncService.CreateAsync(sampleCoffee);
        await asyncService.CreateAsync(sampleTea);
        System.Console.WriteLine($"[CreateAsync] Створено 2 початкові напої. У сервісі: {asyncService.Count} шт.");

        // 2. ReadAsync
        var readItem = await asyncService.ReadAsync(sampleCoffee.Id);
        System.Console.WriteLine($"[ReadAsync] Зчитано напій за Id: {readItem.Name} ({readItem.Price:F2} грн, {readItem.VolumeMl} мл)");

        // 3. UpdateAsync
        sampleCoffee.Price = 135.00m;
        bool updateSuccess = await asyncService.UpdateAsync(sampleCoffee);
        System.Console.WriteLine($"[UpdateAsync] Оновлено ціну до {sampleCoffee.Price:F2} грн. Успіх: {updateSuccess}");

        // 4. RemoveAsync
        bool removeSuccess = await asyncService.RemoveAsync(sampleTea);
        System.Console.WriteLine($"[RemoveAsync] Видалено '{sampleTea.Name}'. Успіх: {removeSuccess}. У сервісі: {asyncService.Count} шт.\n");

        // Очистимо перед масовою генерацією
        asyncService.Clear();

        // -------------------------------------------------------------------------
        // ЧАСТИНА 2. Паралельне створення > 1000 об'єктів з використанням Parallel
        // -------------------------------------------------------------------------
        const int totalItemsToGenerate = 1500;
        System.Console.WriteLine($"--- ЧАСТИНА 2. ПАРАЛЕЛЬНЕ СТВОРЕННЯ {totalItemsToGenerate} ОБ'ЄКТІВ (КЛАС PARALLEL) ---");
        System.Console.WriteLine($"Використовуємо CrudServiceAsync, метод Beverage.CreateNew() та Parallel.For...");

        var sw = Stopwatch.StartNew();

        // Використовуємо Parallel.For для багатопотокового наповнення
        Parallel.For(0, totalItemsToGenerate, _ =>
        {
            var newBeverage = Beverage.CreateNew();
            // CreateAsync є thread-safe завдяки ConcurrentDictionary
            asyncService.CreateAsync(newBeverage).GetAwaiter().GetResult();
        });

        sw.Stop();

        System.Console.WriteLine($"-> Успішно згенеровано та додано {asyncService.Count} об'єктів за {sw.ElapsedMilliseconds} мс.");
        System.Console.WriteLine($"-> Thread-Safety перевірка: Очікувалось = {totalItemsToGenerate}, Фактично = {asyncService.Count} " +
                                 (asyncService.Count == totalItemsToGenerate ? "[УСПІХ: Втрат даних немає!]" : "[ПОМИЛКА: Втрата даних!]"));
        System.Console.WriteLine();

        // -------------------------------------------------------------------------
        // ЧАСТИНА 3. LINQ: Пошук min, max та average для числових значень
        // -------------------------------------------------------------------------
        System.Console.WriteLine("--- ЧАСТИНА 3. LINQ АНАЛІЗ ЦИФРОВИХ ПОКАЗНИКІВ КОЛЕКЦІЇ ---");

        // Оскільки CrudServiceAsync реалізує IEnumerable<T>, виконуємо LINQ безпосередньо по сервісу!
        int totalCount = asyncService.Count();
        decimal minPrice = asyncService.Min(b => b.Price);
        decimal maxPrice = asyncService.Max(b => b.Price);
        decimal avgPrice = asyncService.Average(b => b.Price);

        int minVolume = asyncService.Min(b => b.VolumeMl);
        int maxVolume = asyncService.Max(b => b.VolumeMl);
        double avgVolume = asyncService.Average(b => b.VolumeMl);

        // Показники для специфічних категорій напоїв
        var coffeeList = asyncService.OfType<CoffeeBeverage>().ToList();
        var teaList = asyncService.OfType<TeaBeverage>().ToList();
        var baseBeveragesCount = totalCount - coffeeList.Count - teaList.Count;

        System.Console.WriteLine($"Всього напоїв у системі: {totalCount:N0}");
        System.Console.WriteLine($"  - Кавових напоїв (CoffeeBeverage): {coffeeList.Count:N0} ({coffeeList.Count * 100.0 / totalCount:F1}%)");
        System.Console.WriteLine($"  - Чайних напоїв (TeaBeverage):     {teaList.Count:N0} ({teaList.Count * 100.0 / totalCount:F1}%)");
        System.Console.WriteLine($"  - Інших напоїв (Beverage):        {baseBeveragesCount:N0} ({baseBeveragesCount * 100.0 / totalCount:F1}%)\n");

        System.Console.WriteLine("Статистика цін (Price, грн):");
        System.Console.WriteLine($"  * Мінімальна ціна: {minPrice:F2} грн");
        System.Console.WriteLine($"  * Максимальна ціна: {maxPrice:F2} грн");
        System.Console.WriteLine($"  * Середня ціна:     {avgPrice:F2} грн\n");

        System.Console.WriteLine("Статистика об'ємів (VolumeMl, мл):");
        System.Console.WriteLine($"  * Мінімальний об'єм: {minVolume} мл");
        System.Console.WriteLine($"  * Максимальний об'єм: {maxVolume} мл");
        System.Console.WriteLine($"  * Середній об'єм:     {avgVolume:F1} мл\n");

        if (coffeeList.Any())
        {
            int minCaffeine = coffeeList.Min(c => c.CaffeineAmountMg);
            int maxCaffeine = coffeeList.Max(c => c.CaffeineAmountMg);
            double avgCaffeine = coffeeList.Average(c => c.CaffeineAmountMg);

            System.Console.WriteLine("Специфічні показники для кави (вміст кофеїну, CaffeineAmountMg):");
            System.Console.WriteLine($"  * Мін. кофеїн: {minCaffeine} мг");
            System.Console.WriteLine($"  * Макс. кофеїн: {maxCaffeine} мг");
            System.Console.WriteLine($"  * Середній кофеїн: {avgCaffeine:F1} мг\n");
        }

        if (teaList.Any())
        {
            int minTemp = teaList.Min(t => t.BrewingTemperatureC);
            int maxTemp = teaList.Max(t => t.BrewingTemperatureC);
            double avgTemp = teaList.Average(t => t.BrewingTemperatureC);

            System.Console.WriteLine("Специфічні показники для чаю (температура заварювання, °C):");
            System.Console.WriteLine($"  * Мін. температура: {minTemp}°C");
            System.Console.WriteLine($"  * Макс. температура: {maxTemp}°C");
            System.Console.WriteLine($"  * Середня температура: {avgTemp:F1}°C\n");
        }

        // -------------------------------------------------------------------------
        // ЧАСТИНА 4. Пагінація: ReadAllAsync(page, amount)
        // -------------------------------------------------------------------------
        System.Console.WriteLine("--- ЧАСТИНА 4. ДЕМОНСТРАЦІЯ ПАГІНАЦІЇ (ReadAllAsync(page, amount)) ---");
        const int pageSize = 5;

        for (int page = 1; page <= 2; page++)
        {
            System.Console.WriteLine($"Сторінка #{page} (по {pageSize} елементів):");
            var pagedItems = await asyncService.ReadAllAsync(page, pageSize);
            int itemIndex = 1;
            foreach (var item in pagedItems)
            {
                string typeBadge = item is CoffeeBeverage ? "[Кава]" : item is TeaBeverage ? "[Чай]" : "[Напій]";
                System.Console.WriteLine($"  {itemIndex++}. {typeBadge,-7} \"{item.Name}\" | {item.VolumeMl} мл | {item.Price:F2} грн | ID: {item.Id}");
            }
            System.Console.WriteLine();
        }

        // -------------------------------------------------------------------------
        // ЧАСТИНА 5. Асинхронне збереження колекції у файл (SaveAsync) та завантаження
        // -------------------------------------------------------------------------
        System.Console.WriteLine("--- ЧАСТИНА 5. ЗБЕРЕЖЕННЯ КОЛЕКЦІЇ У ФАЙЛ (SaveAsync) ТА ЗАВАНТАЖЕННЯ ---");
        System.Console.WriteLine($"Збереження у файл: {dataFilePath}");

        sw.Restart();
        bool saveSuccess = await asyncService.SaveAsync();
        sw.Stop();

        var fileInfo = new FileInfo(dataFilePath);
        System.Console.WriteLine($"-> Колекцію успішно збережено: {saveSuccess}");
        System.Console.WriteLine($"-> Розмір серіалізованого JSON-файлу: {fileInfo.Length / 1024.0:F2} КБ ({fileInfo.Length:N0} байт)");
        System.Console.WriteLine($"-> Час асинхронного збереження: {sw.ElapsedMilliseconds} мс.\n");

        System.Console.WriteLine("Перевірка відновлення даних з файлу в новий екземпляр сервісу...");
        var verificationService = new CrudServiceAsync<Beverage>(dataFilePath);
        await verificationService.LoadAsync();
        System.Console.WriteLine($"-> Новий сервіс успішно завантажив з файлу {verificationService.Count} об'єктів. Відповідність 100%!\n");

        // -------------------------------------------------------------------------
        // ЧАСТИНА 6. Приклади використання примітивів синхронізації
        // -------------------------------------------------------------------------
        await SynchronizationDemo.RunAllDemosAsync();
    }
}
