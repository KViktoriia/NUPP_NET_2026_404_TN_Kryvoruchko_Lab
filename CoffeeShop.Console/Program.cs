using System.Text;
using CoffeeShop.Common.Extensions;
using CoffeeShop.Common.Models;
using CoffeeShop.Common.Services;

Console.OutputEncoding = Encoding.UTF8;

Console.WriteLine("=== ДЕМОНСТРАЦІЯ РОБОТИ CRUD СЕРВІСУ ТА МОДЕЛІ КАВ'ЯРНІ ===\n");

// Створення екземплярів
var espresso = new CoffeeBeverage("Еспресо Допіо", 65.00m, 60, "Ефіопія Іргачіф", "Без молока", 130, false);
var cappuccino = new CoffeeBeverage("Капучино", 95.00m, 250, "Колумбія Супремо", "Вівсяне молоко", 85, true);
var matchaLatte = new TeaBeverage("Матча Латте", 110.00m, 300, "Японська Матча Удзі", 75, 2, true);

// Метод розширення
Console.WriteLine("--- Метод розширення ---");
Console.WriteLine(espresso.ToFormattedReceiptString());
Console.WriteLine();

// Статичне поле / метод
Console.WriteLine($"Загальна кількість створених елементів у системі (статичне поле): {Beverage.GetTotalBeveragesCreated()}\n");

var service = new CrudService<Beverage>();

// 1. Create
Console.WriteLine("--- 1. Додавання напоїв до сервісу (Create) ---");
service.Create(espresso);
service.Create(cappuccino);
service.Create(matchaLatte);
Console.WriteLine("Напої успішно додані.\n");

// 2. ReadAll
Console.WriteLine("--- 2. Перегляд усіх напоїв (ReadAll) ---");
PrintBeverageList(service.ReadAll());
Console.WriteLine();

// 3. Read
Console.WriteLine($"--- 3. Отримання напою за ID (Read): {cappuccino.Id} ---");
var found = service.Read(cappuccino.Id);
Console.WriteLine(FormatItem(found));
Console.WriteLine();

// Подія
Console.WriteLine("--- Виклик методу зі зміною ціни (тригер події) ---");
espresso.OnPriceChanged += (bev, oldP, newP) =>
{
    Console.WriteLine($"[ПОДІЯ]: Напій '{bev.Name}' змінив ціну: {oldP:F2} грн -> {newP:F2} грн.");
};
espresso.ChangePrice(75.00m);
Console.WriteLine();

// 4. Update
Console.WriteLine("--- 4. Оновлення напою (Update) ---");
cappuccino.Price = 120.00m;
cappuccino.VolumeMl = 350;
service.Update(cappuccino);
Console.WriteLine($"Оновлений напій: {FormatItem(cappuccino)}\n");

// 5. Remove
Console.WriteLine($"--- 5. Видалення напою (Remove): {matchaLatte.Name} ---");
service.Remove(matchaLatte);
Console.WriteLine("Залишились у списку після видалення:");
PrintBeverageList(service.ReadAll());
Console.WriteLine();

// 6. Save / Load
Console.WriteLine("--- 6. Додаткове завдання: Збереження та завантаження з файлу ---");
string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "coffeeshop_menu.json");
service.Save(filePath);
Console.WriteLine("Дані сервісу успішно збережено у файл 'coffeeshop_menu.json'.");

var newService = new CrudService<Beverage>();
newService.Load(filePath);
Console.WriteLine("Дані, зчитані з файлу в новий екземпляр сервісу:");
PrintBeverageList(newService.ReadAll());

// Допоміжні методи для друку у стилі списку
static void PrintBeverageList(IEnumerable<Beverage> items)
{
    foreach (var item in items)
    {
        Console.WriteLine(FormatItem(item));
    }
}

static string FormatItem(Beverage b)
{
    string tag = b is CoffeeBeverage ? "[Кава]" : b is TeaBeverage ? "[Чай]" : "[Напій]";
    string details = b switch
    {
        CoffeeBeverage c => $"\"{c.Name}\" - {c.CoffeeBeansOrigin} ({c.MilkType}), {c.VolumeMl} мл, {c.Price:F2} грн, Доступний: {c.IsAvailable}",
        TeaBeverage t => $"\"{t.Name}\" - {t.TeaVariety}, {t.VolumeMl} мл, {t.Price:F2} грн, Доступний: {t.IsAvailable}",
        _ => $"\"{b.Name}\", {b.VolumeMl} мл, {b.Price:F2} грн, Доступний: {b.IsAvailable}"
    };
    return $"{tag} ID: {b.Id} | {details}";
}
