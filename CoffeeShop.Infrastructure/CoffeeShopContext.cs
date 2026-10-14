using Microsoft.EntityFrameworkCore;
using CoffeeShop.Infrastructure.Models;

namespace CoffeeShop.Infrastructure;

/// <summary>
/// Контекст бази даних CoffeeShop, що використовує Entity Framework Core та SQLite
/// </summary>
public class CoffeeShopContext : DbContext
{
    public DbSet<BeverageModel> Beverages { get; set; } = null!;
    public DbSet<CoffeeBeverageModel> CoffeeBeverages { get; set; } = null!;
    public DbSet<TeaBeverageModel> TeaBeverages { get; set; } = null!;
    public DbSet<BaristaModel> Baristas { get; set; } = null!;
    public DbSet<OrderReceiptModel> OrderReceipts { get; set; } = null!;
    public DbSet<OrderBeverageModel> OrderBeverages { get; set; } = null!;

    private readonly string _dbPath = string.Empty;

    public CoffeeShopContext()
    {
        _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "coffeeshop.db");
    }

    public CoffeeShopContext(DbContextOptions<CoffeeShopContext> options) : base(options)
    {
    }

    public CoffeeShopContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite($"Data Source={_dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---- TPT (Table-per-Type) для ієрархії напоїв ----
        modelBuilder.Entity<BeverageModel>(entity =>
        {
            entity.ToTable("Beverages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            entity.Property(e => e.VolumeMl).IsRequired();
            entity.Property(e => e.IsAvailable).HasDefaultValue(true);
        });

        modelBuilder.Entity<CoffeeBeverageModel>(entity =>
        {
            entity.ToTable("CoffeeBeverages"); // TPT: окрема таблиця для кави
            entity.Property(e => e.CoffeeBeansOrigin).HasMaxLength(200);
            entity.Property(e => e.MilkType).HasMaxLength(100);
        });

        modelBuilder.Entity<TeaBeverageModel>(entity =>
        {
            entity.ToTable("TeaBeverages"); // TPT: окрема таблиця для чаю
            entity.Property(e => e.TeaVariety).HasMaxLength(200);
        });

        // ---- Barista ----
        modelBuilder.Entity<BaristaModel>(entity =>
        {
            entity.ToTable("Baristas");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Shift).HasMaxLength(50);
            entity.Property(e => e.HourlyRate).HasColumnType("decimal(10,2)");

            // Один-до-багатьох: Barista -> OrderReceipts
            entity.HasMany(e => e.Orders)
                  .WithOne(o => o.Barista)
                  .HasForeignKey(o => o.BaristaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ---- OrderReceipt ----
        modelBuilder.Entity<OrderReceiptModel>(entity =>
        {
            entity.ToTable("OrderReceipts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReceiptNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CustomerName).HasMaxLength(200);
        });

        // ---- Багато-до-багатьох: OrderReceipt <-> Beverage через OrderBeverage ----
        modelBuilder.Entity<OrderBeverageModel>(entity =>
        {
            entity.ToTable("OrderBeverages");
            entity.HasKey(ob => new { ob.OrderReceiptId, ob.BeverageId });

            entity.HasOne(ob => ob.OrderReceipt)
                  .WithMany(o => o.OrderBeverages)
                  .HasForeignKey(ob => ob.OrderReceiptId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ob => ob.Beverage)
                  .WithMany(b => b.OrderBeverages)
                  .HasForeignKey(ob => ob.BeverageId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
