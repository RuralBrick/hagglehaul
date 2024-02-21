namespace hagglehaul.Server.Models
{
    public class DriverTripInfo
    {
        public string TripId { get; set; }
        public string RiderEmail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public double PickupLong { get; set; }
        public double PickupLat { get; set; }
        public double DestinationLong { get; set; }
        public double DestinationLat { get; set; }
        public uint? PartySize { get; set; } = null!;
    }
}
