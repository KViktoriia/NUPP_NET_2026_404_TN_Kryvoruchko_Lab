using DbBarista = CoffeeShop.Infrastructure.Models.BaristaModel;
using DbBeverage = CoffeeShop.Infrastructure.Models.BeverageModel;

namespace CoffeeShop.REST.Models;

/// <summary>
/// Методи розширення для перетворення між моделями БД та моделями API
/// </summary>
public static class MappingExtensions
{
    public static BaristaModel ToDto(this DbBarista entity)
    {
        return new BaristaModel
        {
            Id = entity.Id,
            FullName = entity.FullName,
            ExperienceYears = entity.ExperienceYears,
            Shift = entity.Shift,
            HourlyRate = entity.HourlyRate,
            IsCertified = entity.IsCertified
        };
    }

    public static DbBarista ToEntity(this CreateBaristaModel model)
    {
        return new DbBarista
        {
            FullName = model.FullName,
            ExperienceYears = model.ExperienceYears,
            Shift = model.Shift,
            HourlyRate = model.HourlyRate,
            IsCertified = model.IsCertified
        };
    }

    public static void UpdateEntity(this UpdateBaristaModel model, DbBarista entity)
    {
        entity.FullName = model.FullName;
        entity.ExperienceYears = model.ExperienceYears;
        entity.Shift = model.Shift;
        entity.HourlyRate = model.HourlyRate;
        entity.IsCertified = model.IsCertified;
    }

    public static BeverageModel ToDto(this DbBeverage entity)
    {
        return new BeverageModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            VolumeMl = entity.VolumeMl,
            IsAvailable = entity.IsAvailable
        };
    }

    public static DbBeverage ToEntity(this CreateBeverageModel model)
    {
        return new DbBeverage
        {
            Name = model.Name,
            Price = model.Price,
            VolumeMl = model.VolumeMl,
            IsAvailable = model.IsAvailable
        };
    }

    public static void UpdateEntity(this UpdateBeverageModel model, DbBeverage entity)
    {
        entity.Name = model.Name;
        entity.Price = model.Price;
        entity.VolumeMl = model.VolumeMl;
        entity.IsAvailable = model.IsAvailable;
    }
}
