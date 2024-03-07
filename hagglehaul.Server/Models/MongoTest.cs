using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models;

/// <summary>
/// Database model for testing MongoDB.
/// </summary>
[BsonIgnoreExtraElements]
public class MongoTest
{
    /// <summary>
    /// The unique identifier of the document.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// The test string.
    /// </summary>
    [BsonElement("test")]
    public string Test { get; set; } = null!;
}
