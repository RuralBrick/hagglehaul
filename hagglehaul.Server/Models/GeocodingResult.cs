using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models;

/// <summary>
/// The representation of a cached geocoding result in the database.
/// </summary>
[BsonIgnoreExtraElements]
public class GeocodingResult
{
    /// <summary>
    /// The unique identifier of the geocoding result, not used in the application.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    /// <summary>
    /// The search string that was used to obtain the geocoding result.
    /// </summary>
    [BsonElement("query")]
    public string Query { get; set; } = null!;
    
    /// <summary>
    /// The JSON object, stored as a string, that represents the geocoding result.
    /// </summary>
    [BsonElement("features")]
    public string Features { get; set; } = null!;
}
