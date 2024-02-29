namespace hagglehaul.Server.Models
{
    public class ConfirmedRiderTrip
    {
        public string TripID { get; set; } = null!;
        public string TripName { get; set; } = null!;
        public byte[] Thumbnail { get; set; } = null!;
        public string GeoJson { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public double? Distance { get; set; } //distance in meters
        public double? Duration { get; set; } //duration in seconds
        public uint Cost { get; set; }
        public string DriverName { get; set; } = null!;
        public double? DriverRating { get; set; }
        public uint? DriverNumRating { get; set; }
        public bool ShowRatingPrompt { get; set; }
        public string DriverEmail { get; set; } = null!;
        public string DriverPhone { get; set; } = null!;
        public string PickupAddress { get; set; } = null!;
        public string DestinationAddress { get; set; } = null!;
        public string DriverCarModel { get; set; } = null!;
    }
}
