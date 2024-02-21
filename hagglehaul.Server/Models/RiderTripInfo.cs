using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace hagglehaul.Server.Models
{
    public class RiderTripInfo
    {
        public string TripId { get; set; }
        public string DriverEmail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public double PickupLong { get; set; }
        public double PickupLat { get; set; }
        public double DestinationLong { get; set; }
        public double DestinationLat { get; set; }
        public uint? PartySize { get; set; } = null!;
    }
}
