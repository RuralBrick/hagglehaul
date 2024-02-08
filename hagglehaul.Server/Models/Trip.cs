using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    public class Trip
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("riderEmail")]
        public string RiderEmail { get; set; } = null!;

        [BsonElement("driverEmail")]
        public string DriverEmail { get; set; } = null!;

        [BsonElement("routeId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string RouteId { get; set; } = null!;
    }
}
