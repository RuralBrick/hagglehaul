namespace hagglehaul.Server.Models
{
    public class BidUserView
    {
        public string? BidId { get; set; }
        public bool YourBid { get; set; }
        public string DriverName { get; set; } = null!;
        public double? DriverRating { get; set; }
        public uint? DriverNumRating { get; set; }
        public uint Cost { get; set; }
    }
}
