using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    public class DriverProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("rating")]
        public int Rating { get; set; }

        [BsonElement("carDescription")]
        public string CarDescription { get; set; } = null!;
    }
}
