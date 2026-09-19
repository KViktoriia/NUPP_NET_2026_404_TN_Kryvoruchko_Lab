using CoffeeShop.Common.Interfaces;

namespace CoffeeShop.Common.Models;

// Клас баристи (персонал кав'ярні)
public class Barista : IEntity
{
    // Властивості
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public int ExperienceYears { get; set; }
    public string Shift { get; set; } = "Ранкова";
    public decimal HourlyRate { get; set; }
    public bool IsCertified { get; set; }

    // конструктор (за замовчуванням)
    public Barista()
    {
        Id = Guid.NewGuid();
    }

    // конструктор (параметризований)
    public Barista(string fullName, int experienceYears, string shift, decimal hourlyRate, bool isCertified)
    {
        Id = Guid.NewGuid();
        FullName = fullName;
        ExperienceYears = experienceYears;
        Shift = shift;
        HourlyRate = hourlyRate;
        IsCertified = isCertified;
    }

    // метод
    public string GetInfo()
    {
        return $"Бариста: {FullName}, Досвід: {ExperienceYears} р., Зміна: {Shift}, Ставка: {HourlyRate} грн/год, Сертифікація: {(IsCertified ? "Має" : "Немає")}";
    }

    // метод
    public decimal CalculateSalary(int workedHours)
    {
        return workedHours * HourlyRate;
    }
}
