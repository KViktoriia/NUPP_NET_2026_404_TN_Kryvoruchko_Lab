using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CoffeeShop.Infrastructure;
using CoffeeShop.Infrastructure.Repositories;
using CoffeeShop.Infrastructure.Services;
using DbBarista = CoffeeShop.Infrastructure.Models.BaristaModel;
using DbBeverage = CoffeeShop.Infrastructure.Models.BeverageModel;
using DbOrderReceipt = CoffeeShop.Infrastructure.Models.OrderReceiptModel;

var builder = WebApplication.CreateBuilder(args);

// 1. Додавання сервісів контролерів
builder.Services.AddControllers();

// 2. Налаштування бази даних SQLite (CoffeeShopContext)
string dbPath = Path.Combine(builder.Environment.ContentRootPath, "coffeeshop.db");
if (!File.Exists(dbPath))
{
    string parentDb = Path.Combine(builder.Environment.ContentRootPath, "..", "coffeeshop.db");
    if (File.Exists(parentDb))
    {
        File.Copy(parentDb, dbPath, true);
    }
}

builder.Services.AddDbContext<CoffeeShopContext>(options =>
{
    options.UseSqlite($"Data Source={dbPath}");
});

// 3. Реєстрація репозиторіїв у контейнері впровадження залежностей (DI)
builder.Services.AddScoped<IRepository<DbBarista>, Repository<DbBarista>>();
builder.Services.AddScoped<IRepository<DbBeverage>, Repository<DbBeverage>>();
builder.Services.AddScoped<IRepository<DbOrderReceipt>, Repository<DbOrderReceipt>>();

// 4. Реєстрація асинхронних CRUD-сервісів у контейнері DI
builder.Services.AddScoped<ICrudServiceAsync<DbBarista>, CrudServiceDbAsync<DbBarista>>();
builder.Services.AddScoped<ICrudServiceAsync<DbBeverage>, CrudServiceDbAsync<DbBeverage>>();

builder.Services.AddScoped<ICrudServiceDbAsync<DbBarista>, CrudServiceDbAsync<DbBarista>>();
builder.Services.AddScoped<ICrudServiceDbAsync<DbBeverage>, CrudServiceDbAsync<DbBeverage>>();

// 5. Налаштування Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoffeeShop REST API",
        Version = "v1",
        Description = "RESTful API для керування кав'ярнею CoffeeShop (Лабораторна робота №4)",
        Contact = new OpenApiContact
        {
            Name = "Student Developer",
            Email = "student@nupp.edu.ua"
        }
    });
});

var app = builder.Build();

// 6. Ініціалізація та наповнення БД початковими даними при першому старті
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoffeeShopContext>();
    await context.Database.EnsureCreatedAsync();

    if (!await context.Baristas.AnyAsync())
    {
        context.Baristas.AddRange(
            new DbBarista
            {
                FullName = "Олександр Коваленко",
                ExperienceYears = 4,
                Shift = "Ранок",
                HourlyRate = 160.00m,
                IsCertified = true
            },
            new DbBarista
            {
                FullName = "Марія Шевченко",
                ExperienceYears = 2,
                Shift = "Вечір",
                HourlyRate = 135.50m,
                IsCertified = false
            },
            new DbBarista
            {
                FullName = "Дмитро Мельник",
                ExperienceYears = 5,
                Shift = "Ранок",
                HourlyRate = 180.00m,
                IsCertified = true
            }
        );
        await context.SaveChangesAsync();
    }

    if (!await context.Beverages.AnyAsync())
    {
        context.Beverages.AddRange(
            new DbBeverage
            {
                Name = "Капучино Класичний",
                Price = 75.00m,
                VolumeMl = 250,
                IsAvailable = true
            },
            new DbBeverage
            {
                Name = "Еспресо Допіо",
                Price = 55.00m,
                VolumeMl = 60,
                IsAvailable = true
            },
            new DbBeverage
            {
                Name = "Чорний чай з бергамотом",
                Price = 60.00m,
                VolumeMl = 350,
                IsAvailable = true
            },
            new DbBeverage
            {
                Name = "Лате Карамель",
                Price = 85.00m,
                VolumeMl = 300,
                IsAvailable = true
            }
        );
        await context.SaveChangesAsync();
    }
}

// 7. Конфігурація конвеєра обробки HTTP-запитів
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeShop REST API v1");
    c.RoutePrefix = string.Empty; // Swagger на кореневій URL: http://localhost:PORT/
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
