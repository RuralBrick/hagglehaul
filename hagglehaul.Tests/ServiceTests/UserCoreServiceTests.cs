using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;

namespace hagglehaul.Tests.ServiceTests;

public class UserCoreServiceTests : ServiceTestsBase
{
    [SetUp]
    public void Setup()
    {
        _database.CreateCollection("UserCore");
    }
    
    [TearDown]
    public void TearDown()
    {
        _database.DropCollection("UserCore");
    }
    
    [Test]
    public async Task CanCreateAndDeleteUserCore()
    {
        var userCoreService = new UserCoreService(_database);
        
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.CreateAsync(HhTestUtilities.GetUserCoreData(1).First()));
        var actual = await userCoreService.GetAsync(HhTestUtilities.GetUserCoreData(1).First().Email);
        Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetUserCoreData(1).First(), actual));
        
        await userCoreService.RemoveAsync(HhTestUtilities.GetUserCoreData(1).First().Email);
        actual = await userCoreService.GetAsync(HhTestUtilities.GetUserCoreData(1).First().Email);
        Assert.IsNull(actual);
    }

    [Test]
    public async Task CanGetUserCoreByEmail()
    {
        var userCoreService = new UserCoreService(_database);
        
        var data = HhTestUtilities.GetUserCoreData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.CreateAsync(data));
        var actual = await userCoreService.GetAsync(data.Email);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
    }

    [Test]
    public async Task CanGetMultipleUserCoreObjects()
    {
        var userCoreService = new UserCoreService(_database);
        
        var data = HhTestUtilities.GetUserCoreData(2);
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.CreateAsync(data.First()));
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.CreateAsync(data.Last()));
        var actual = await userCoreService.GetAsync();
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
    }

    [Test]
    public async Task CanUpdateUserCoreObject()
    {
        var userCoreService = new UserCoreService(_database);
        
        var data = HhTestUtilities.GetUserCoreData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.CreateAsync(data));
        var actual = await userCoreService.GetAsync(data.Email);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
        
        data.Name = "New Name";
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.UpdateAsync(data.Email, data));
        actual = await userCoreService.GetAsync(data.Email);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
    }
    
    [Test]
    public async Task CanDeleteUserCoreObject()
    {
        var userCoreService = new UserCoreService(_database);
        
        var data = HhTestUtilities.GetUserCoreData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await userCoreService.CreateAsync(data));
        var actual = await userCoreService.GetAsync(data.Email);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));
        
        await userCoreService.RemoveAsync(data.Email);
        actual = await userCoreService.GetAsync(data.Email);
        Assert.IsNull(actual);
    }

    [Test]
    public async Task HashFunctionSanity()
    {
        var userCoreService = new UserCoreService(_database);
        var password = "Xtw9NMgx";
        var salt = "CGYzqeN4plZekNC88Umm1Q==";
        var hash = "Gt9Yc4AiIvmsC1QQbe2RZsCIqvoYlst2xbz0Fs8aHnw=";

        var incorrectPasswords = new List<string>
        {
            "Xtw9NMgy",
            "xtw9NMgx",
            "Xtw9NMgx ",
            "Xtw9NMgx\n",
            " Xtw9NMgx",
            "\nXtw9NMgx",
            "Xtw9NMgxXtw9NMgx",
            "xtw9nmgx"
        };

        Assert.IsTrue(userCoreService.ComparePasswordToHash(password, hash, salt));

        foreach (var incorrectPassword in incorrectPasswords)
        {
            Assert.IsFalse(userCoreService.ComparePasswordToHash(incorrectPassword, hash, salt));
        }

        string newHash = String.Empty;
        string newSalt = String.Empty;
        Assert.DoesNotThrow(() => userCoreService.CreatePasswordHash(password, out newHash, out newSalt));
        Assert.IsTrue(userCoreService.ComparePasswordToHash(password, newHash, newSalt));
        
        foreach (var incorrectPassword in incorrectPasswords)
        {
            Assert.IsFalse(userCoreService.ComparePasswordToHash(incorrectPassword, newHash, newSalt));
        }
    }
}
