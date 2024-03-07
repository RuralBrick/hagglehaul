using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The representation of a driver's profile in the database.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class DriverProfile
    {
        /// <summary>
        /// The ID of the driver's profile. Not used in the client.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// The email address of the driver.
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// The rating of the driver, from 1.0 to 5.0.
        /// </summary>
        [BsonElement("rating")]
        public double? Rating { get; set; } = null!;
        
        /// <summary>
        /// The number of ratings the driver has received.
        /// </summary>
        [BsonElement("numRatings")]
        public uint? NumRatings { get; set; } = null!;

        /// <summary>
        /// The make, model, year, and license plate of the driver's car.
        /// </summary>
        [BsonElement("carDescription")]
        public string CarDescription { get; set; } = null!;
    }
}
