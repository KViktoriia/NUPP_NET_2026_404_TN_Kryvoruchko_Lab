using CoffeeShop.Infrastructure.Models;
using CoffeeShop.Infrastructure.Repositories;
using MongoDB.Driver;

namespace CoffeeShop.Nosql.Repositories;

/// <summary>
/// Репозиторій для нереляційної бази даних MongoDB, що реалізує інтерфейс IRepository<T>
/// </summary>
/// <typeparam name="T">Тип документа, що реалізує IEntityModel</typeparam>
public class MongoRepository<T> : IRepository<T> where T : class, IEntityModel
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string? collectionName = null)
    {
        collectionName ??= typeof(T).Name.ToLowerInvariant() + "s";
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task Update(T entity)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task Delete(T entity)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        await _collection.DeleteOneAsync(filter);
    }

    public Task SaveChangesAsync()
    {
        // У MongoDB операції запису виконуються одразу
        return Task.CompletedTask;
    }
}
