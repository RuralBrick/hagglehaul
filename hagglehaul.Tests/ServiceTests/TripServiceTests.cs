using hagglehaul.Server.Services;
using hagglehaul.Tests.SharedHelpers;

namespace hagglehaul.Tests.ServiceTests;

public class TripServiceTests : ServiceTestsBase
{
    [SetUp]
    public void Setup()
    {
        _database.CreateCollection("Trip");
    }
    
    [TearDown]
    public void TearDown()
    {
        _database.DropCollection("Trip");
    }
    
    [Test]
    public async Task CanCreateAndDeleteTrip()
    {
        var tripService = new TripService(_database);
        
        var trip = HhTestUtilities.GetTripData(1).First();
        Assert.DoesNotThrowAsync( async () =>  await tripService.CreateAsync(trip));
        var actual = await tripService.GetTripByIdAsync(trip.Id);
        trip.StartTime = actual.StartTime; // non-trivial to compare due to marshalling
        Assert.IsTrue(HhTestUtilities.CompareJson(trip, actual));
        
        await tripService.DeleteAsync(trip.Id);
        actual = await tripService.GetTripByIdAsync(trip.Id);
        Assert.IsNull(actual);
    }

    [Test]
    public async Task CanGetTripsByRiderEmail()
    {
        var tripService = new TripService(_database);
        var riderEmail = "rider@example.com";
        var trips = HhTestUtilities.GetTripData(2);

        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trips.First()));
        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trips.Last()));
        
        var actual = await tripService.GetRiderTripsAsync(riderEmail);
        actual.First().StartTime = trips.First().StartTime; // non-trivial to compare due to marshalling
        actual.Last().StartTime = trips.Last().StartTime; // non-trivial to compare due to marshalling
        Assert.IsTrue(HhTestUtilities.CompareJson(trips, actual));
    }

    [Test]
    public async Task CanGetTripsByDriverEmail()
    {
        var tripService = new TripService(_database);
        var driverEmail = "driver@example.com";
        var trips = HhTestUtilities.GetTripData(2, hasDriver:true);
        
        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trips.First()));
        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trips.Last()));
        
        var actual = await tripService.GetDriverTripsAsync(driverEmail);
        actual.First().StartTime = trips.First().StartTime; // non-trivial to compare due to marshalling
        actual.Last().StartTime = trips.Last().StartTime; // non-trivial to compare due to marshalling
        Assert.IsTrue(HhTestUtilities.CompareJson(trips, actual));
    }
    
    [Test]
    public async Task CanGetAllTrips()
    {
        var tripService = new TripService(_database);
        var trips = HhTestUtilities.GetTripData(2);
        
        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trips.First()));
        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trips.Last()));
        
        var actual = await tripService.GetAllTripsAsync();
        actual.First().StartTime = trips.First().StartTime; // non-trivial to compare due to marshalling
        actual.Last().StartTime = trips.Last().StartTime; // non-trivial to compare due to marshalling
        Assert.IsTrue(HhTestUtilities.CompareJson(trips, actual));
    }
    
    [Test]
    public async Task CanUpdateTrip()
    {
        var tripService = new TripService(_database);
        var trip = HhTestUtilities.GetTripData(1).First();
        Assert.DoesNotThrowAsync(async () => await tripService.CreateAsync(trip));
        
        var actual = await tripService.GetTripByIdAsync(trip.Id);
        actual.StartTime = trip.StartTime; // non-trivial to compare due to marshalling
        Assert.IsTrue(HhTestUtilities.CompareJson(trip, actual));
        
        trip.StartTime = DateTime.Now;
        await tripService.UpdateAsync(trip.Id, trip);
        actual = await tripService.GetTripByIdAsync(trip.Id);
        actual.StartTime = trip.StartTime; // non-trivial to compare due to marshalling
        Assert.IsTrue(HhTestUtilities.CompareJson(trip, actual));
    }
}
