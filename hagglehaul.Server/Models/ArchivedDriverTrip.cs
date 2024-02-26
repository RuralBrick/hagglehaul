namespace hagglehaul.Server.Models
{
    public class ArchivedDriverTrip
    {
        public string TripId { get; set; } = null!;
        public string TripName { get; set; } = null!;
        public byte[] Thumbnail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public double? Distance { get; set; } //distance in meters
        public double? Duration { get; set; } //duration in seconds
        public uint Cost { get; set; }
        public string RiderName { get; set; } = null!;
        public double? RiderRating { get; set; }
        public uint? RiderNumRating { get; set; }
        public string PickupAddress { get; set; } = null!;
        public string DestinationAddress { get; set; } = null!;
    }
}
