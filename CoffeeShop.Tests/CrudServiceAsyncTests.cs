using CoffeeShop.Common.Models;
using CoffeeShop.Common.Services;
using Xunit;

namespace CoffeeShop.Tests;

public class CrudServiceAsyncTests
{
    private readonly string _testFilePath;

    public CrudServiceAsyncTests()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"coffeeshop_test_{Guid.NewGuid():N}.json");
    }

    [Fact]
    public async Task CreateAsync_ValidEntity_ShouldAddAndReturnTrue()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var coffee = new CoffeeBeverage("Лате", 85.00m, 350, "Колумбія", "Вівсяне", 90, false);

        // Act
        bool created = await service.CreateAsync(coffee);
        var read = await service.ReadAsync(coffee.Id);

        // Assert
        Assert.True(created);
        Assert.Equal(1, service.Count);
        Assert.Equal("Лате", read.Name);
    }

    [Fact]
    public async Task CreateAsync_DuplicateId_ShouldReturnFalse()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var beverage = new Beverage("Лимонад", 60.00m, 300);

        // Act
        bool first = await service.CreateAsync(beverage);
        bool second = await service.CreateAsync(beverage);

        // Assert
        Assert.True(first);
        Assert.False(second);
        Assert.Equal(1, service.Count);
    }

    [Fact]
    public async Task CreateAsync_NullEntity_ShouldThrowArgumentNullException()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateAsync(null!));
    }

    [Fact]
    public void CreateAsync_MultithreadedParallelExecution_ShouldBeThreadSafe()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        const int itemsCount = 1000;

        // Act
        Parallel.For(0, itemsCount, _ =>
        {
            var item = Beverage.CreateNew();
            service.CreateAsync(item).GetAwaiter().GetResult();
        });

        // Assert
        Assert.Equal(itemsCount, service.Count);
    }

    [Fact]
    public async Task ReadAsync_ExistingEntity_ShouldReturnEntity()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var tea = new TeaBeverage("Матча", 100.00m, 250, "Удзі", 80, 2, true);
        await service.CreateAsync(tea);

        // Act
        var result = await service.ReadAsync(tea.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tea.Id, result.Id);
        Assert.Equal("Матча", result.Name);
    }

    [Fact]
    public async Task ReadAsync_NonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var nonExistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ReadAsync(nonExistentId));
    }

    [Fact]
    public async Task ReadAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        for (int i = 0; i < 5; i++)
        {
            await service.CreateAsync(Beverage.CreateNew());
        }

        // Act
        var all = await service.ReadAllAsync();

        // Assert
        Assert.Equal(5, all.Count());
    }

    [Fact]
    public async Task ReadAllAsync_WithPagination_ShouldReturnCorrectPageAndAmount()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        for (int i = 0; i < 15; i++)
        {
            await service.CreateAsync(new Beverage($"Напій #{i}", 50m + i, 200 + i * 10));
        }

        // Act
        var page1 = (await service.ReadAllAsync(page: 1, amount: 5)).ToList();
        var page2 = (await service.ReadAllAsync(page: 2, amount: 5)).ToList();
        var page3 = (await service.ReadAllAsync(page: 3, amount: 5)).ToList();
        var page4 = (await service.ReadAllAsync(page: 4, amount: 5)).ToList();

        // Assert
        Assert.Equal(5, page1.Count);
        Assert.Equal(5, page2.Count);
        Assert.Equal(5, page3.Count);
        Assert.Empty(page4); // Немає елементів на 4-й сторінці

        // Перевіряємо що сторінки не перетинаються
        var page1Ids = page1.Select(x => x.Id).ToHashSet();
        Assert.True(page2.All(x => !page1Ids.Contains(x.Id)));
    }

    [Fact]
    public async Task ReadAllAsync_InvalidPageOrAmount_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ReadAllAsync(page: 0, amount: 5));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.ReadAllAsync(page: 1, amount: -1));
    }

    [Fact]
    public async Task UpdateAsync_ExistingEntity_ShouldUpdateAndReturnTrue()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var coffee = new CoffeeBeverage("Капучино", 70.00m, 250, "Бразилія", "Коров'яче", 80, false);
        await service.CreateAsync(coffee);

        // Act
        coffee.Price = 85.00m;
        coffee.MilkType = "Мигдальне";
        bool updated = await service.UpdateAsync(coffee);
        var reRead = (CoffeeBeverage)await service.ReadAsync(coffee.Id);

        // Assert
        Assert.True(updated);
        Assert.Equal(85.00m, reRead.Price);
        Assert.Equal("Мигдальне", reRead.MilkType);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingEntity_ShouldReturnFalse()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var coffee = new CoffeeBeverage("Капучино", 70.00m, 250, "Бразилія", "Коров'яче", 80, false);

        // Act
        bool updated = await service.UpdateAsync(coffee);

        // Assert
        Assert.False(updated);
    }

    [Fact]
    public async Task RemoveAsync_ExistingEntity_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var beverage = Beverage.CreateNew();
        await service.CreateAsync(beverage);

        // Act
        bool removed = await service.RemoveAsync(beverage);

        // Assert
        Assert.True(removed);
        Assert.Equal(0, service.Count);
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ReadAsync(beverage.Id));
    }

    [Fact]
    public async Task RemoveAsync_NonExistingEntity_ShouldReturnFalse()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var beverage = Beverage.CreateNew();

        // Act
        bool removed = await service.RemoveAsync(beverage);

        // Assert
        Assert.False(removed);
    }

    [Fact]
    public async Task SaveAsync_And_LoadAsync_ShouldPersistAndRestoreCollection()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        var coffee = new CoffeeBeverage("Еспресо", 55.00m, 60, "Ефіопія", "Без молока", 120, false);
        var tea = new TeaBeverage("Сенча", 65.00m, 300, "Японія", 80, 3, false);
        await service.CreateAsync(coffee);
        await service.CreateAsync(tea);

        // Act
        bool saved = await service.SaveAsync();

        var newService = new CrudServiceAsync<Beverage>(_testFilePath);
        bool loaded = await newService.LoadAsync();

        // Assert
        Assert.True(saved);
        Assert.True(loaded);
        Assert.True(File.Exists(_testFilePath));
        Assert.Equal(2, newService.Count);

        var restoredCoffee = (CoffeeBeverage)await newService.ReadAsync(coffee.Id);
        Assert.Equal("Еспресо", restoredCoffee.Name);
        Assert.Equal("Ефіопія", restoredCoffee.CoffeeBeansOrigin);

        // Cleanup
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public async Task IEnumerable_ShouldSupportLinqQueriesDirectlyOnService()
    {
        // Arrange
        var service = new CrudServiceAsync<Beverage>(_testFilePath);
        await service.CreateAsync(new Beverage("Чай", 40.00m, 200));
        await service.CreateAsync(new Beverage("Кава", 60.00m, 250));
        await service.CreateAsync(new Beverage("Смузі", 80.00m, 350));

        // Act (LINQ)
        decimal min = service.Min(x => x.Price);
        decimal max = service.Max(x => x.Price);
        decimal avg = service.Average(x => x.Price);
        int count = service.Count();

        // Assert
        Assert.Equal(3, count);
        Assert.Equal(40.00m, min);
        Assert.Equal(80.00m, max);
        Assert.Equal(60.00m, avg);
    }

    [Fact]
    public void FactoryMethods_CreateNew_ShouldProduceValidPopulatedObjects()
    {
        // Act
        var beverage = Beverage.CreateNew();
        var coffee = CoffeeBeverage.CreateNew();
        var tea = TeaBeverage.CreateNew();
        var barista = Barista.CreateNew();
        var receipt = OrderReceipt.CreateNew();

        // Assert
        Assert.NotNull(beverage);
        Assert.NotEqual(Guid.Empty, beverage.Id);
        Assert.False(string.IsNullOrWhiteSpace(beverage.Name));
        Assert.True(beverage.Price > 0);

        Assert.NotNull(coffee);
        Assert.False(string.IsNullOrWhiteSpace(coffee.CoffeeBeansOrigin));
        Assert.True(coffee.CaffeineAmountMg > 0);

        Assert.NotNull(tea);
        Assert.False(string.IsNullOrWhiteSpace(tea.TeaVariety));
        Assert.True(tea.BrewingTemperatureC > 0);

        Assert.NotNull(barista);
        Assert.False(string.IsNullOrWhiteSpace(barista.FullName));
        Assert.True(barista.HourlyRate > 0);

        Assert.NotNull(receipt);
        Assert.False(string.IsNullOrWhiteSpace(receipt.ReceiptNumber));
        Assert.True(receipt.TotalAmount > 0);
    }
}
