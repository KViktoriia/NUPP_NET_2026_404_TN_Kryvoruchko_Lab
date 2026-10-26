using CoffeeShop.Infrastructure.Models;
using CoffeeShop.Nosql.Repositories;
using Mongo2Go;
using MongoDB.Driver;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("=============================================================");
Console.WriteLine("  ЗАПУСК ЛОКАЛЬНОГО СЕРВЕРА MONGODB ДЛЯ MONGODB COMPASS      ");
Console.WriteLine("=============================================================\n");

// Mongo2Go автоматично виділяє вільний порт та запускає локальний mongod
using var runner = MongoDbRunner.Start();
Console.WriteLine($"[OK] Сервер MongoDB успішно запущено!");
Console.WriteLine($"[URI для підключення]: {runner.ConnectionString}\n");

var client = new MongoClient(runner.ConnectionString);
var database = client.GetDatabase("coffeeshop_nosql");

var beverageRepo = new MongoRepository<BeverageModel>(database, "beverages");
var baristaRepo = new MongoRepository<BaristaModel>(database, "baristas");

// Наповнюємо тестовими даними
await baristaRepo.AddAsync(new BaristaModel
{
    Id = 1,
    FullName = "Олександр Коваленко",
    ExperienceYears = 4,
    Shift = "Ранкова",
    HourlyRate = 160.00m,
    IsCertified = true
});

await baristaRepo.AddAsync(new BaristaModel
{
    Id = 2,
    FullName = "Марія Шевченко",
    ExperienceYears = 2,
    Shift = "Денна",
    HourlyRate = 135.50m,
    IsCertified = false
});

await beverageRepo.AddAsync(new CoffeeBeverageModel
{
    Id = 1,
    Name = "Капучино Преміум",
    Price = 75.00m,
    VolumeMl = 250,
    CoffeeBeansOrigin = "Ефіопія Іргачіф",
    MilkType = "Вівсяне Barista",
    CaffeineAmountMg = 95,
    HasSyrup = true,
    IsAvailable = true
});

await beverageRepo.AddAsync(new TeaBeverageModel
{
    Id = 2,
    Name = "Матча Лате",
    Price = 90.00m,
    VolumeMl = 300,
    TeaVariety = "Японська Сенча",
    BrewingTemperatureC = 80,
    SteepingTimeMinutes = 3,
    HasHoney = true,
    IsAvailable = true
});

await beverageRepo.AddAsync(new BeverageModel
{
    Id = 3,
    Name = "Лимонад Цитрусовий",
    Price = 65.00m,
    VolumeMl = 400,
    IsAvailable = true
});

Console.WriteLine("[OK] Колекції 'baristas' та 'beverages' успішно наповнено даними!");
Console.WriteLine("-------------------------------------------------------------");
Console.WriteLine("ЯК ПІДКЛЮЧИТИСЯ В MONGODB COMPASS:");
Console.WriteLine($"1. Скопіюйте оцей рядок: {runner.ConnectionString}");
Console.WriteLine("2. Вставте його у верхнє поле в Compass і натисніть 'Connect'.");
Console.WriteLine("3. Зліва відкрийте базу 'coffeeshop_nosql' -> колекцію 'beverages'.");
Console.WriteLine("-------------------------------------------------------------");
Console.WriteLine("Сервер зараз працює! Зробіть скріншот у Compass.");
Console.WriteLine("Коли закінчите, натисніть Enter у цій консолі для зупинки...");
Console.ReadLine();
