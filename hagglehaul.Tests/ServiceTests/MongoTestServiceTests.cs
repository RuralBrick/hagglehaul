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

    [Test]
    public async Task CanGetMongoTestById()
    {
        var mongoTestService = new MongoTestService(_database);

        var data = HhTestUtilities.GetMongoTestData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await mongoTestService.CreateAsync(data));
        var actual = await mongoTestService.GetAsync(data.Id);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
    }

    [Test]
    public async Task CanUpdateAndRemoveMongoTest()
    {
        var mongoTestService = new MongoTestService(_database);
        var data = HhTestUtilities.GetMongoTestData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await mongoTestService.CreateAsync(data));
        var actual = await mongoTestService.GetAsync(data.Id);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
        
        data.Test = "Updated";
        Assert.DoesNotThrowAsync( async () =>  await mongoTestService.UpdateAsync(data.Id, data));
        actual = await mongoTestService.GetAsync(data.Id);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
        
        Assert.DoesNotThrowAsync( async () =>  await mongoTestService.RemoveAsync(data.Id));
        actual = await mongoTestService.GetAsync(data.Id);
        Assert.IsNull(actual);
    }
}