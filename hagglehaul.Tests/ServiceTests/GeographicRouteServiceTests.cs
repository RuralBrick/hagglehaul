using hagglehaul.Server.Models;
using hagglehaul.Server.Services;
using Microsoft.Extensions.Options;

namespace hagglehaul.Tests.ServiceTests;

public class GeographicRouteServiceTests : ServiceTestsBase
{
    protected IOptions<MapboxSettings> _mapboxSettings;
    // TODO: find way to keep this access token secret
    protected const string ACCESS_TOKEN = "";
    
    [SetUp]
    public void Setup()
    {
        _mapboxSettings = Options.Create(new MapboxSettings {AccessToken = ACCESS_TOKEN});
        
        _database.CreateCollection("RouteInfoCache");
    }
    
    [TearDown]
    public void TearDown()
    {
        _database.DropCollection("RouteInfoCache");
    }

    [Test]
    public async Task GeographicRouteCachesProperly()
    {
        if (String.IsNullOrEmpty(ACCESS_TOKEN)) Assert.Ignore("Mapbox access token not set -- skipping Mapbox-related tests");
        var geographicRouteService = new GeographicRouteService(_database, _mapboxSettings);
        
        var route = await geographicRouteService.GetGeographicRoute(-73.9876, 40.7661, -73.9642, 40.7926);
        Assert.IsNotNull(route);
        Assert.That(route.Distance, Is.GreaterThan(100));
        Assert.That(route.Duration, Is.GreaterThan(60));
        
        var cacheRoute = await geographicRouteService.GetGeographicRoute(-73.98761, 40.76611, -73.96421, 40.79261);
        Assert.IsNotNull(cacheRoute);
        Assert.That(route.Distance, Is.EqualTo(cacheRoute.Distance));
        Assert.That(route.Duration, Is.EqualTo(cacheRoute.Duration));
    }
}
