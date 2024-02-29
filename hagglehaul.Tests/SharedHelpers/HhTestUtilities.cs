using System.Text;
using System.Text.Json;
using hagglehaul.Server.Models;

namespace hagglehaul.Tests.SharedHelpers;

public class HhTestUtilities
{
    public static bool CompareJson(object obj1, object obj2)
    {
        var json1 = JsonSerializer.Serialize(obj1);
        var json2 = JsonSerializer.Serialize(obj2);
        return json1 == json2;
    }

    public static List<MongoTest> GetMongoTestData(int count = 2)
    {
        var result = new List<MongoTest>(count);
        for (var i = 1; i <= count; i++)
        {
            result.Add(new MongoTest
            {
                Id = new StringBuilder().Insert(0, i.ToString(), 24).ToString(),
                Test = $"Test {i}"
            });
        }

        return result;
    }

    public static List<RiderProfile> GetRiderProfileData(int count = 2)
    {
        var result = new List<RiderProfile>(count);
        for (var i = 1; i <= count; i++)
        {
            result.Add(new RiderProfile
            {
                Id = new StringBuilder().Insert(0, i.ToString(), 24).ToString(),
                Email = $"Test{i}@email.com"
            });
        }

        return result;
    }

    public static List<DriverProfile> GetDriverProfileData(int count = 2)
    {
        var result = new List<DriverProfile>(count);
        for (var i = 1; i <= count; i++)
        {
            result.Add(new DriverProfile
            {
                Id = new StringBuilder().Insert(0, i.ToString(), 24).ToString(),
                Email = $"Test{i}@email.com"
            });
        }

        return result;
    }

    public static List<Trip> GetTripData(int count = 2, bool inPast = false, bool hasDriver = false, int timeDelta = 0)
    {
        var result = new List<Trip>(count);
        for (var i = 1; i <= count; i++)
        {
            result.Add(new Trip
            {
                Id = new StringBuilder().Insert(0, i.ToString(), 24).ToString(),
                RiderEmail = "rider@example.com",
                DriverEmail = hasDriver ? "driver@example.com" : null,
                Name = $"MyTrip{i}",
                StartTime = DateTime.Now.AddHours(inPast ? (-2 - i * timeDelta) : (36 + i * timeDelta)),
                PickupLong = 1.0 + (i * 0.1),
                PickupLat = 1.0 + (i * 0.1),
                DestinationLong = 2.0 + (i * 0.1),
                DestinationLat = 2.0 + (i * 0.1),
                PickupAddress = "123 Main St",
                DestinationAddress = "456 Elm St",
                PartySize = 3,
                RiderHasBeenRated = false,
                DriverHasBeenRated = false
            });
        }

        return result;
    }

    public static List<Bid> GetBidData(int count = 2, bool sameTrip = false)
    {
        var result = new List<Bid>(count);
        for (var i = 1; i <= count; i++)
        {
            result.Add(new Bid
            {
                Id = new StringBuilder().Insert(0, i.ToString(), 24).ToString(),
                DriverEmail = "driver@example.com",
                TripId = new StringBuilder().Insert(0, (sameTrip? 1 : i).ToString(), 24).ToString(),
                CentsAmount = 100
            });
        }

        return result;
    }
}
