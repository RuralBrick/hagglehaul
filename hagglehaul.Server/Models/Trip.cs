using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    public class Trip
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }

        // [maybe] Name (string)
        // Route (ref to RouteCache)
        // Bid list (ref to Bid)
        // Rider that posted trip (ref to UserCore)
        // Confirmed driver (ref to UserCore)
    }
}
