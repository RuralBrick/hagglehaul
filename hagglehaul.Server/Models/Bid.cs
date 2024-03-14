using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The representation of a bid in the database.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Bid
    {
        /// <summary>
        /// The ID of the bid.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// The email of the driver who created the bid.
        /// </summary>
        [BsonElement("driverEmail")]
        public string DriverEmail { get; set; } = null!;
        
        /// <summary>
        /// The Trip ID of the trip the bid is for.
        /// </summary>
        [BsonElement("tripId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string TripId { get; set; } = null!;

        /// <summary>
        /// The cost, in cents, of the bid.
        /// </summary>
        [BsonElement("centsAmount")]
        public uint CentsAmount { get; set; }
    }
}
