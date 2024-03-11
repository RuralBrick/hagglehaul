using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// The representation of a user in the database.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class UserCore
    {
        /// <summary>
        /// The unique identifier of the user, not used in the application.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        [BsonElement("email")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// The phone number of the user.
        /// </summary>
        [BsonElement("phone")]
        public string Phone { get; set; } = null!;

        /// <summary>
        /// The name of the user.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// The salt used to hash the password, as a base64 string.
        /// </summary>
        [BsonElement("salt")]
        public string Salt { get; set; } = null!;

        /// <summary>
        /// The hash of the password, as a base64 string.
        /// </summary>
        [BsonElement("passwordHash")]
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// The role of the user, must be either "rider" or "driver".
        /// </summary>
        [BsonElement("role")]
        public string Role { get; set; } = null!;
    }
}
