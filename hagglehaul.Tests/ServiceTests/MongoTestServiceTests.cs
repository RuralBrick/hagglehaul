using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;

namespace hagglehaul.Tests.ServiceTests;

public class MongoTestServiceTests : ServiceTestsBase
{
    [SetUp]
    public void Setup()
    {
        _database.CreateCollection("MongoTest");
    }
    
    [TearDown]
    public void TearDown()
    {
        _database.DropCollection("MongoTest");
    }
    
    [Test]
    public async Task CanCreateAndDeleteMongoTest()
    {
        var mongoTestService = new MongoTestService(_database);
        
        Assert.DoesNotThrowAsync( async () =>  await mongoTestService.CreateAsync(HhTestUtilities.GetMongoTestData(1).First()));
        var actual = await mongoTestService.GetAsync();
        Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetMongoTestData(1), actual));
    }
}