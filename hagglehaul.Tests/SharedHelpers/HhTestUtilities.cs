using System.Text.Json;

namespace hagglehaul.Tests.SharedHelpers;

public class HhTestUtilities
{
    public static bool CompareJson(object obj1, object obj2)
    {
        var json1 = JsonSerializer.Serialize(obj1);
        var json2 = JsonSerializer.Serialize(obj2);
        return json1 == json2;
    }
}