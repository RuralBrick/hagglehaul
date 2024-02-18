using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    public class RiderBasicInfo
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
    }
}
