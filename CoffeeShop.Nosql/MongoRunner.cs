using CoffeeShop.Infrastructure.Models;
using CoffeeShop.Nosql.Repositories;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

namespace CoffeeShop.Nosql;

public class MongoRunner
{
    public static async Task RunAndExportJsonAsync(string outputJsonPath)
    {
        using var runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var database = client.GetDatabase("coffeeshop_nosql");

        var beverageRepo = new MongoRepository<BeverageModel>(database, "beverages");
        var baristaRepo = new MongoRepository<BaristaModel>(database, "baristas");

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

        var bevCollection = database.GetCollection<BsonDocument>("beverages");
        var baristaCollection = database.GetCollection<BsonDocument>("baristas");

        var bevDocs = await bevCollection.Find(new BsonDocument()).ToListAsync();
        var baristaDocs = await baristaCollection.Find(new BsonDocument()).ToListAsync();

        var jsonSettings = new JsonWriterSettings { Indent = true };

        using var writer = new StreamWriter(outputJsonPath, false, System.Text.Encoding.UTF8);
        writer.WriteLine("// === DATABASE: coffeeshop_nosql | COLLECTION: baristas ===");
        foreach (var doc in baristaDocs)
        {
            writer.WriteLine(doc.ToJson(jsonSettings));
        }

        writer.WriteLine("\n// === DATABASE: coffeeshop_nosql | COLLECTION: beverages ===");
        foreach (var doc in bevDocs)
        {
            writer.WriteLine(doc.ToJson(jsonSettings));
        }
    }
}
