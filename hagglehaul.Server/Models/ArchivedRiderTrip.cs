namespace hagglehaul.Server.Models
{
    public class ArchivedRiderTrip
    {
        public string TripID { get; set; } = null!;
        public string TripName { get; set; } = null!;
        public byte[] Thumbnail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public uint Distance { get; set; } //distance in meters
        public uint Duration { get; set; } //duration in seconds
        public bool HasDriver { get; set; } //below data invalid if False
        public uint Cost { get; set; }
        public string DriverName { get; set; } = null!;
        public double DriverRating { get; set; }
        public uint DriverNumRating { get; set; }
        public string PickupAddress { get; set; } = null!;
        public string DestinationAddress { get; set; } = null!;
    }
}
