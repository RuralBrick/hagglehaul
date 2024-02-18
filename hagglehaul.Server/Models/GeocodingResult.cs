using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models;

[BsonIgnoreExtraElements]
public class GeocodingResult
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("query")]
    public string Query { get; set; } = null!;
    
    [BsonElement("features")]
    public string Features { get; set; } = null!;
}
