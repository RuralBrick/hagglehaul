namespace hagglehaul.Server.Models
{
    public class UnconfirmedRiderTrip
    {
        public string TripID { get; set; } = null!;
        public string TripName { get; set; } = null!;
        public byte[] Thumbnail { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public double? Distance { get; set; } //distance in meters
        public double? Duration { get; set; } //duration in seconds
        public List<Bid>? Bids { get; set; }
        public string PickupAddress { get; set; } = null!;
        public string DestinationAddress { get; set; } = null!;
    }
}
