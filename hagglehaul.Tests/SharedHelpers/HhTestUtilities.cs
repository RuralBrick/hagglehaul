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
}
