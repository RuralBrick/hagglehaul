using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services;

/// <summary>
/// A test class to ensure MongoDB is working properly.
/// </summary>
public interface IMongoTestService
{
    Task<List<MongoTest>> GetAsync();
    Task<MongoTest> GetAsync(string id);
    Task UpdateAsync(string id, MongoTest mongoTestIn);
    Task RemoveAsync(string id);
}

/// <summary>
/// See <see cref="IMongoTestService"/>.
/// </summary>
public class MongoTestService : IMongoTestService
{
    private readonly IMongoCollection<MongoTest> _mongoTestCollection;
    
    public MongoTestService(IMongoDatabase database)
    {
        _mongoTestCollection = database.GetCollection<MongoTest>("MongoTest");
    }
    
    public async Task<List<MongoTest>> GetAsync() =>
        await _mongoTestCollection.Find(_ => true).ToListAsync();
    
    public async Task<MongoTest> GetAsync(string id) =>
        await _mongoTestCollection.Find(mongoTest => mongoTest.Id == id).FirstOrDefaultAsync();
    
    public async Task CreateAsync(MongoTest mongoTest) =>
        await _mongoTestCollection.InsertOneAsync(mongoTest);
    
    public async Task UpdateAsync(string id, MongoTest mongoTestIn) =>
        await _mongoTestCollection.ReplaceOneAsync(mongoTest => mongoTest.Id == id, mongoTestIn);
    
    public async Task RemoveAsync(string id) =>
        await _mongoTestCollection.DeleteOneAsync(mongoTest => mongoTest.Id == id);
}
