using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models;

[BsonIgnoreExtraElements]
public class GeographicRoute
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("routeDescriptor")]
    public string RouteDescriptor { get; set; } = null!;
    
    [BsonElement("image")]
    public byte[] Image { get; set; } = null!;
    
    [BsonElement("distance")]
    public double? Distance { get; set; } = null!;
    
    [BsonElement("duration")]
    public double? Duration { get; set; } = null!;
    
    [BsonElement("geoJson")]
    public string GeoJson { get; set; } = null!;
}
