using hagglehaul.Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace hagglehaul.Server.Services;
public class MongoTestService
{
    private readonly IMongoCollection<MongoTest> _mongoTestCollection;
    
    public MongoTestService(IOptions<HagglehaulDatabaseSettings> hagglehaulDatabaseSettings)
    {
        var client = new MongoClient(hagglehaulDatabaseSettings.Value.ConnectionString);
        var database = client.GetDatabase(hagglehaulDatabaseSettings.Value.DatabaseName);
        _mongoTestCollection = database.GetCollection<MongoTest>("MongoTest");
    }
    
    public async Task<List<MongoTest>> GetAsync() =>
        await _mongoTestCollection.Find(_ => true).ToListAsync();
    
    public async Task<MongoTest> GetAsync(string id) =>
        await _mongoTestCollection.Find(mongoTest => mongoTest.Id == id).FirstOrDefaultAsync();
    
    public async Task UpdateAsync(string id, MongoTest mongoTestIn) =>
        await _mongoTestCollection.ReplaceOneAsync(mongoTest => mongoTest.Id == id, mongoTestIn);
    
    public async Task RemoveAsync(string id) =>
        await _mongoTestCollection.DeleteOneAsync(mongoTest => mongoTest.Id == id);
}