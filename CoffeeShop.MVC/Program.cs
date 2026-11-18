using Microsoft.EntityFrameworkCore;
using CoffeeShop.Infrastructure;
using CoffeeShop.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database configuration
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

var app = builder.Build();

// Ensure DB is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CoffeeShopContext>();
    await context.Database.EnsureCreatedAsync();

    if (!await context.Baristas.AnyAsync())
    {
        context.Baristas.AddRange(
            new BaristaModel
            {
                FullName = "Олександр Коваленко",
                ExperienceYears = 4,
                Shift = "Ранок",
                HourlyRate = 160.00m,
                IsCertified = true
            },
            new BaristaModel
            {
                FullName = "Марія Шевченко",
                ExperienceYears = 2,
                Shift = "Вечір",
                HourlyRate = 135.50m,
                IsCertified = false
            },
            new BaristaModel
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
            new BeverageModel
            {
                Name = "Капучино Класичний",
                Price = 75.00m,
                VolumeMl = 250,
                IsAvailable = true
            },
            new BeverageModel
            {
                Name = "Еспресо Допіо",
                Price = 55.00m,
                VolumeMl = 60,
                IsAvailable = true
            },
            new BeverageModel
            {
                Name = "Чорний чай з бергамотом",
                Price = 60.00m,
                VolumeMl = 350,
                IsAvailable = true
            },
            new BeverageModel
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
