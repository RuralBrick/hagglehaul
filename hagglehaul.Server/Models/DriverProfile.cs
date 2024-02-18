using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    [BsonIgnoreExtraElements]
    public class DriverProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("rating")]
        public double? Rating { get; set; } = null!;
        
        [BsonElement("numRatings")]
        public uint? NumRatings { get; set; } = null!;

        [BsonElement("carDescription")]
        public string CarDescription { get; set; } = null!;
    }
}
