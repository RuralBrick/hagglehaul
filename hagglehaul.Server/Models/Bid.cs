using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    public class Bid
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("centsAmount")]
        public uint CentsAmount { get; set; }

        // Rider (ref to rider that placed bid)
        // Trip (ref to trip bid is placed under)
    }
}
