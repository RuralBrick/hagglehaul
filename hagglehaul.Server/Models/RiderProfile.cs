using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The database model for a rider's profile.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class RiderProfile
    {
        /// <summary>
        /// The unique identifier of the rider, not used in the application.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// The email address of the rider.
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// The rating of the rider, ranging from 1.0 to 5.0.
        /// </summary>
        [BsonElement("rating")]
        public double? Rating { get; set; } = null!;
        
        /// <summary>
        /// The number of ratings the rider has received.
        /// </summary>
        [BsonElement("numRatings")]
        public uint? NumRatings { get; set; } = null!;
    }
}
