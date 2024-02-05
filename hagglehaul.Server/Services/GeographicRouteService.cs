using hagglehaul.Server.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Bson;

namespace hagglehaul.Server.Services;

public interface IGeographicRouteService
{
    Task<GeographicRoute> GetGeographicRoute(double startLong, double startLat, double endLong, double endLat);
}

public class GeographicRouteService : IGeographicRouteService
{
    private readonly IMongoCollection<GeographicRoute> _geographicRouteCollection;
    private readonly MapboxSettings _mapboxSettings;
    
    public GeographicRouteService(IMongoDatabase database, IOptions<MapboxSettings> mapboxSettings)
    {
        _geographicRouteCollection = database.GetCollection<GeographicRoute>("RouteInfoCache");
        _mapboxSettings = mapboxSettings.Value;
    }
    
    protected const string ACCENT_COLOR = "#D96C06";
    protected const string MAP_STYLE = "light-v11";
    protected const string IMAGE_WIDTH = "600";
    protected const string IMAGE_HEIGHT = "400";
    
    protected static string CreateGeoJsonPoint(double longitude, double latitude, string name, string color) =>
        $"{{\"type\":\"Feature\",\"geometry\":{{\"type\":\"Point\",\"coordinates\":[{longitude},{latitude}]}},\"properties\":{{\"name\":\"{name}\",\"marker-color\":\"{color}\"}}}}";
    
    protected static string CreateGeoJsonRouteObject(string point1, string point2, string routeGeometry, string routeColor) =>
        $"{{\"type\":\"FeatureCollection\",\"features\":[{point1},{point2},{{\"type\":\"Feature\",\"geometry\":{routeGeometry},\"properties\":{{\"stroke\":\"{routeColor}\",\"name\":\"Route\"}}}}]}}";
    
    public async Task<GeographicRoute> GetGeographicRoute(double startLong, double startLat, double endLong, double endLat)
    {
        string startLongString = startLong.ToString("0.0000");
        string startLatString = startLat.ToString("0.0000");
        string endLongString = endLong.ToString("0.0000");
        string endLatString = endLat.ToString("0.0000");
        
        string routeDescriptor = $"{startLongString},{startLatString};{endLongString},{endLatString}";
        var route = await _geographicRouteCollection.Find(route => route.RouteDescriptor == routeDescriptor).FirstOrDefaultAsync();
        if (route == null)
        {
            route = new GeographicRoute
            {
                Id = ObjectId.GenerateNewId().ToString(),
                RouteDescriptor = routeDescriptor
            };
            // Call Mapbox API to get route information
            using (HttpClient client = new HttpClient())
            {
                // Part 1: Get navigation from navigation API
                string encodedRouteDescriptor = UrlEncoder.Default.Encode(routeDescriptor);
                string navigationUrl = $"https://api.mapbox.com/directions/v5/mapbox/driving/{encodedRouteDescriptor}";
                var navigationParameters = new Dictionary<string, string?>
                {
                    { "alternatives", "false" },
                    { "geometries", "geojson" },
                    { "overview", "full" },
                    { "steps", "false" },
                    { "access_token", _mapboxSettings.AccessToken }
                };
                navigationUrl = QueryHelpers.AddQueryString(navigationUrl, navigationParameters);
                Console.WriteLine($"GET {navigationUrl}");
                
                try
                {
                    HttpResponseMessage response = await client.GetAsync(navigationUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        JsonDocument jsonResponse = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        if (jsonResponse.RootElement.TryGetProperty("routes", out JsonElement routesElement))
                        {
                            JsonElement routeElement = routesElement.EnumerateArray().FirstOrDefault();
                            if (routeElement.TryGetProperty("geometry", out JsonElement geometryElement) &&
                                routeElement.TryGetProperty("distance", out JsonElement distanceElement) &&
                                routeElement.TryGetProperty("duration", out JsonElement durationElement))
                            {
                                route.GeoJson = geometryElement.ToString()!;
                                route.Distance = distanceElement.GetDouble();
                                route.Duration = durationElement.GetDouble();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Mapbox navigation response is missing required attributes");
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Mapbox navigation call returned error, status code: {response.StatusCode}");
                        return null;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Mapbox navigation call failed, message: {e.Message} ");
                    return null;
                }

                // Part 2: Get static map image
                string point1 = CreateGeoJsonPoint(startLong, startLat, "P", ACCENT_COLOR);
                string point2 = CreateGeoJsonPoint(endLong, endLat, "D", ACCENT_COLOR);
                string inputGeoJson = CreateGeoJsonRouteObject(point1, point2, route.GeoJson, ACCENT_COLOR);
                string encodedInputGeoJson = UrlEncoder.Default.Encode(inputGeoJson);
                string imageUrl = $"https://api.mapbox.com/styles/v1/mapbox/{MAP_STYLE}/static/geojson({encodedInputGeoJson})/auto/{IMAGE_WIDTH}x{IMAGE_HEIGHT}";
                var imageParameters = new Dictionary<string, string?>
                {
                    { "access_token", _mapboxSettings.AccessToken }
                };
                imageUrl = QueryHelpers.AddQueryString(imageUrl, imageParameters);
                Console.WriteLine($"GET {imageUrl}");
                
                try
                {
                    HttpResponseMessage response = await client.GetAsync(imageUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        route.Image = await response.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Mapbox static map call returned error, status code: {response.StatusCode}");
                        return null;
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Mapbox static map call failed, message: {e.Message} ");
                    return null;
                }
            }

            // Save route information to database
            await _geographicRouteCollection.InsertOneAsync(route);
        }
        return route;
    }
}
