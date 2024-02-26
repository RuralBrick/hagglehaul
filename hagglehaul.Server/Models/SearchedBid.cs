namespace hagglehaul.Server.Models
{
    public class SearchedBid
    {
        public string DriverName { get; set; }
        public double? DriverRating { get; set; }
        public uint? DriverNumRatings { get; set; }
        public uint CentsAmount { get; set; }
    }
}
