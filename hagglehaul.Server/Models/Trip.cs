using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Database model for a trip.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class Trip
    {
        /// <summary>
        /// The unique identifier of the trip.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// The email address of the rider who requested the trip.
        /// </summary>
        [BsonElement("riderEmail")]
        public string RiderEmail { get; set; } = null!;

        /// <summary>
        /// The email address of the driver who was selected for the trip.
        /// </summary>
        [BsonElement("driverEmail")]
        public string DriverEmail { get; set; } = null!;

        /// <summary>
        /// The name of the trip.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The start time of the trip.
        /// </summary>
        [BsonElement("startTime")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The longitude of the pickup location, WGS84.
        /// </summary>
        [BsonElement("pickupLong")]
        public double PickupLong { get; set; }

        /// <summary>
        /// The latitude of the pickup location, WGS84.
        /// </summary>
        [BsonElement("pickupLat")]
        public double PickupLat { get; set; }

        /// <summary>
        /// The longitude of the destination, WGS84.
        /// </summary>
        [BsonElement("destinationLong")]
        public double DestinationLong { get; set; }

        /// <summary>
        /// The latitude of the destination, WGS84.
        /// </summary>
        [BsonElement("destinationLat")]
        public double DestinationLat { get; set; }
        
        /// <summary>
        /// The pickup address of the trip.
        /// </summary>
        [BsonElement("pickupAddress")]
        public string PickupAddress { get; set; } = null!;
        
        /// <summary>
        /// The destination address of the trip.
        /// </summary>
        [BsonElement("destinationAddress")]
        public string DestinationAddress { get; set; } = null!;

        /// <summary>
        /// The number of riders in the trip.
        /// </summary>
        [BsonElement("partySize")]
        public uint? PartySize { get; set; } = null!;

        /// <summary>
        /// Whether the driver has already rated the rider for this trip.
        /// </summary>
        [BsonElement("riderHasBeenRated")]
        public bool RiderHasBeenRated { get; set; } = false;

        /// <summary>
        /// Whether the rider has already rated the driver for this trip.
        /// </summary>
        [BsonElement("driverHasBeenRated")]
        public bool DriverHasBeenRated { get; set; } = false;
    }
}
