using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models;

[BsonIgnoreExtraElements]
public class MongoTest
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("test")]
    public string Test { get; set; } = null!;
}
