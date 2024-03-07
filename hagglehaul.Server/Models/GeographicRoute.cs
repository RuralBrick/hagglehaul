using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models;

/// <summary>
/// The representation of a cached geographic route in the database.
/// </summary>
[BsonIgnoreExtraElements]
public class GeographicRoute
{
    /// <summary>
    /// The unique identifier of the geographic route, not used in the application.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// The series of coordinates that was used to obtain the geographic route.
    /// Must have 4 float strings, each with exactly 4 decimal places of WGS84.
    /// Format: "startLng,startLat;endLng,endLat".
    /// </summary>
    [BsonElement("routeDescriptor")]
    public string RouteDescriptor { get; set; } = null!;
    
    /// <summary>
    /// The image of the geographic route, stored as a byte array.
    /// </summary>
    [BsonElement("image")]
    public byte[] Image { get; set; } = null!;
    
    /// <summary>
    /// The distance of the route in meters.
    /// </summary>
    [BsonElement("distance")]
    public double? Distance { get; set; } = null!;
    
    /// <summary>
    /// The duration of the route in seconds.
    /// </summary>
    [BsonElement("duration")]
    public double? Duration { get; set; } = null!;
    
    /// <summary>
    /// GeoJSON representation of the route.
    /// </summary>
    [BsonElement("geoJson")]
    public string GeoJson { get; set; } = null!;
}
