using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;

namespace hagglehaul.Tests.ServiceTests;

public class BidServiceTests : ServiceTestsBase
{
    [SetUp]
    public void Setup()
    {
        _database.CreateCollection("Bid");
    }
    
    [TearDown]
    public void TearDown()
    {
        _database.DropCollection("Bid");
    }

    [Test]
    public async Task CanCreateAndDeleteBid()
    {
        var bidService = new BidService(_database);
        
        Assert.DoesNotThrowAsync( async () =>  await bidService.CreateAsync(HhTestUtilities.GetBidData(1).First()));
        var actual = await bidService.GetTripBidsAsync(HhTestUtilities.GetBidData(1).First().TripId);
        Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetBidData(1), actual));
        
        actual = await bidService.GetDriverBidsAsync(HhTestUtilities.GetBidData(1).First().DriverEmail);
        Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetBidData(1), actual));
        
        await bidService.DeleteAsync(HhTestUtilities.GetBidData(1).First().Id);
        actual = await bidService.GetTripBidsAsync(HhTestUtilities.GetBidData(1).First().TripId);
        Assert.IsEmpty(actual);
    }

    [Test]
    public async Task CanGetMultipleBidsAssociatedWithTrip()
    {
        var bidService = new BidService(_database);
        
        var data = HhTestUtilities.GetBidData(2, sameTrip:true);
        Assert.DoesNotThrowAsync( async () =>  await bidService.CreateAsync(data.First()));
        Assert.DoesNotThrowAsync( async () =>  await bidService.CreateAsync(data.Last()));
        var actual = await bidService.GetTripBidsAsync(data.First().TripId);
        Assert.IsTrue(HhTestUtilities.CompareJson(data, actual));

        await bidService.DeleteByTripIdAsync(data.First().TripId);
        actual = await bidService.GetTripBidsAsync(data.First().TripId);
        Assert.IsEmpty(actual);
    }

    [Test]
    public async Task UpdateBidTest()
    {
        var bidService = new BidService(_database);
        var data = HhTestUtilities.GetBidData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await bidService.CreateAsync(data));
        var actual = await bidService.GetTripBidsAsync(data.TripId);
        Assert.IsTrue(HhTestUtilities.CompareJson(HhTestUtilities.GetBidData(1), actual));
        
        data.CentsAmount = 12345;
        Assert.DoesNotThrowAsync( async () =>  await bidService.UpdateAsync(data.Id, data));
        actual = await bidService.GetTripBidsAsync(data.TripId);
        Assert.IsTrue(HhTestUtilities.CompareJson(new List<Bid> {data}, actual));
    }
}
