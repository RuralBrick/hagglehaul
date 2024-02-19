using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    [BsonIgnoreExtraElements]
    public class Trip
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("riderEmail")]
        public string RiderEmail { get; set; } = null!;

        [BsonElement("driverEmail")]
        public string DriverEmail { get; set; } = null!;

        [BsonElement("startTime")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime StartTime { get; set; }

        [BsonElement("pickupLong")]
        public double PickupLong { get; set; }

        [BsonElement("pickupLat")]
        public double PickupLat { get; set; }

        [BsonElement("destinationLong")]
        public double DestinationLong { get; set; }

        [BsonElement("destinationLat")]
        public double DestinationLat { get; set; }

        [BsonElement("partySize")]
        public uint? PartySize { get; set; } = null!;
    }
}
