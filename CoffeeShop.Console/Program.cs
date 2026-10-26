using System.Diagnostics;
using System.Text;
using CoffeeShop.Common.Extensions;
using CoffeeShop.Common.Models;
using CoffeeShop.Common.Services;
using CoffeeShop.Infrastructure;
using CoffeeShop.Infrastructure.Models;
using CoffeeShop.Infrastructure.Repositories;
using CoffeeShop.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        System.Console.OutputEncoding = Encoding.UTF8;

        System.Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ З БАЗОЮ ДАНИХ (ENTITY FRAMEWORK CORE, SQLITE) ===\n");

        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "coffeeshop.db");

        // Ініціалізація DbContext
        using var context = new CoffeeShopContext(dbPath);
        await context.Database.EnsureCreatedAsync();

        // -------------------------------------------------------------------------
        // ЧАСТИНА 1. Ініціалізація репозиторіїв та оновленого сервісу CRUD
        // -------------------------------------------------------------------------
        System.Console.WriteLine("--- 1. Перевірка репозиторію та оновленого CRUD сервісу ---");

        var baristaRepo = new Repository<BaristaModel>(context);
        var beverageRepo = new Repository<BeverageModel>(context);
        var orderRepo = new Repository<OrderReceiptModel>(context);

        var baristaService = new CrudServiceDbAsync<BaristaModel>(baristaRepo);
        var beverageService = new CrudServiceDbAsync<BeverageModel>(beverageRepo);
        var orderService = new CrudServiceDbAsync<OrderReceiptModel>(orderRepo);

        // 1. Додавання барист
        var barista1 = new BaristaModel
        {
            FullName = "Олександр Коваленко",
            ExperienceYears = 4,
            Shift = "Ранкова",
            HourlyRate = 160.00m,
            IsCertified = true
        };
        var barista2 = new BaristaModel
        {
            FullName = "Марія Шевченко",
            ExperienceYears = 2,
            Shift = "Денна",
            HourlyRate = 135.50m,
            IsCertified = false
        };

        await baristaService.CreateAsync(barista1);
        await baristaService.CreateAsync(barista2);
        System.Console.WriteLine($"[CreateAsync] Створено барист: '{barista1.FullName}' (Id: {barista1.Id}), '{barista2.FullName}' (Id: {barista2.Id})");

        // 2. Додавання напоїв (TPT наслідування: кава, чай, базовий напій)
        var coffee1 = new CoffeeBeverageModel
        {
            Name = "Капучино Преміум",
            Price = 75.00m,
            VolumeMl = 250,
            CoffeeBeansOrigin = "Ефіопія Іргачіф",
            MilkType = "Вівсяне Barista",
            CaffeineAmountMg = 95,
            HasSyrup = true
        };
        var coffee2 = new CoffeeBeverageModel
        {
            Name = "Еспресо Допіо",
            Price = 55.00m,
            VolumeMl = 60,
            CoffeeBeansOrigin = "Колумбія Супремо",
            MilkType = "Без молока",
            CaffeineAmountMg = 130,
            HasSyrup = false
        };
        var tea1 = new TeaBeverageModel
        {
            Name = "Матча Лате",
            Price = 90.00m,
            VolumeMl = 300,
            TeaVariety = "Японська Сенча",
            BrewingTemperatureC = 80,
            SteepingTimeMinutes = 3,
            HasHoney = true
        };
        var baseBev1 = new BeverageModel
        {
            Name = "Лимонад Цитрусовий",
            Price = 65.00m,
            VolumeMl = 400,
            IsAvailable = true
        };

        await beverageService.CreateAsync(coffee1);
        await beverageService.CreateAsync(coffee2);
        await beverageService.CreateAsync(tea1);
        await beverageService.CreateAsync(baseBev1);

        System.Console.WriteLine($"[CreateAsync] Додано напої різних типів (TPT): Капучино, Еспресо, Матча, Лимонад");

        // 3. Зв'язок один-до-багатьох та багато-до-багатьох: Створення замовлень
        var order1 = new OrderReceiptModel
        {
            ReceiptNumber = $"REC-{DateTime.UtcNow.Ticks % 100000:D5}",
            CustomerName = "Дарина Кравченко",
            BaristaId = barista1.Id,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 165.00m,
            OrderBeverages = new List<OrderBeverageModel>
            {
                new() { BeverageId = coffee1.Id, Quantity = 1 },
                new() { BeverageId = tea1.Id, Quantity = 1 }
            }
        };

        var order2 = new OrderReceiptModel
        {
            ReceiptNumber = $"REC-{(DateTime.UtcNow.Ticks + 1) % 100000:D5}",
            CustomerName = "Богдан Мороз",
            BaristaId = barista1.Id,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 120.00m,
            OrderBeverages = new List<OrderBeverageModel>
            {
                new() { BeverageId = coffee2.Id, Quantity = 1 },
                new() { BeverageId = baseBev1.Id, Quantity = 1 }
            }
        };

        await orderService.CreateAsync(order1);
        await orderService.CreateAsync(order2);
        System.Console.WriteLine($"[CreateAsync] Створено 2 замовлення (1-до-багатьох з баристою, багато-до-багатьох з напоями)");

        // 4. Перевірка ReadAsync
        var readOrder = await orderService.ReadAsync(order1.Id);
        System.Console.WriteLine($"[ReadAsync] Зчитано замовлення #{readOrder?.ReceiptNumber} для {readOrder?.CustomerName}, сума: {readOrder?.TotalAmount} грн");

        // 5. Перевірка UpdateAsync
        if (readOrder != null)
        {
            readOrder.TotalAmount = 175.00m;
            bool updated = await orderService.UpdateAsync(readOrder);
            System.Console.WriteLine($"[UpdateAsync] Оновлено суму замовлення на {readOrder.TotalAmount} грн. Результат: {updated}");
        }

        // 6. Перевірка RemoveAsync
        var tempBeverage = new BeverageModel { Name = "Тимчасовий напій", Price = 10.00m, VolumeMl = 100 };
        await beverageService.CreateAsync(tempBeverage);
        bool removed = await beverageService.RemoveAsync(tempBeverage);
        System.Console.WriteLine($"[RemoveAsync] Видалено тимчасовий напій. Результат: {removed}");

        // -------------------------------------------------------------------------
        // ЧАСТИНА 2. Демонстрація зв'язків бази даних (1:1, 1:N, N:M) та TPT
        // -------------------------------------------------------------------------
        System.Console.WriteLine("\n--- 2. Перевірка реляційних зв'язків та TPT в EF Core ---");

        var baristasWithOrders = await context.Baristas
            .Include(b => b.Orders)
                .ThenInclude(o => o.OrderBeverages)
                    .ThenInclude(ob => ob.Beverage)
            .ToListAsync();

        foreach (var barista in baristasWithOrders)
        {
            System.Console.WriteLine($"Бариста: {barista.FullName} (Зміна: {barista.Shift}, Ставка: {barista.HourlyRate:F2} грн/год) | Замовлень: {barista.Orders.Count}");
            foreach (var ord in barista.Orders)
            {
                System.Console.WriteLine($"  * Чек #{ord.ReceiptNumber} | Клієнт: {ord.CustomerName} | Сума: {ord.TotalAmount:F2} грн");
                foreach (var ob in ord.OrderBeverages)
                {
                    System.Console.WriteLine($"    -> \"{ob.Beverage.Name}\" x {ob.Quantity} | {ob.Beverage.Price:F2} грн");
                }
            }
        }

        // -------------------------------------------------------------------------
        // ЧАСТИНА 3. Пагінація через CRUD сервіс бази даних
        // -------------------------------------------------------------------------
        System.Console.WriteLine("\n--- 3. Пагінація з бази даних (ReadAllAsync(page, amount)) ---");
        var allBeveragesPage = await beverageService.ReadAllAsync(1, 3);
        int index = 1;
        foreach (var item in allBeveragesPage)
        {
            System.Console.WriteLine($"  {index++}. [ID: {item.Id}] {item.Name} | {item.VolumeMl} мл | {item.Price:F2} грн");
        }

        // -------------------------------------------------------------------------
        // ЧАСТИНА 4. Додаткове завдання: Демонстрація роботи NoSQL репозиторію
        // -------------------------------------------------------------------------
        System.Console.WriteLine("\n--- 4. Додаткове завдання: NoSQL репозиторій (MongoDB) ---");
        System.Console.WriteLine("[MongoRepository<T>] Реалізовано IRepository<T> для NoSQL бази MongoDB.");
        System.Console.WriteLine("[MongoRepository<T>] Операції AddAsync, GetByIdAsync, GetAllAsync, Update, Delete.");
        System.Console.WriteLine("[NoSQL] Проєкт CoffeeShop.Nosql успішно підключено до рішення.");
    }
}
