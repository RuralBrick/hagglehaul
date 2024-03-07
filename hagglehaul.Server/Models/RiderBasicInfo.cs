using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data used to display basic information about a rider.
    /// </summary>
    public class RiderBasicInfo
    {
        /// <summary>
        /// The name of the rider.
        /// </summary>
        public string Name { get; set; } = null!;
        
        /// <summary>
        /// The email address of the rider.
        /// </summary>
        public string Email { get; set; } = null!;
        
        /// <summary>
        /// The phone number of the rider.
        /// </summary>
        public string Phone { get; set; } = null!;
    }
}
