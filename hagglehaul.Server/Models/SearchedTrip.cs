namespace hagglehaul.Server.Models
{
    public class SearchedTrip
    {
        public string TripId { get; set; }
        public string TripName { get; set; }
        public byte[] Thumbnail { get; set; }
        public string GeoJson { get; set; }
        public DateTime StartTime { get; set; }
        public double? Distance { get; set; }
        public double? Duration { get; set; }
        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }
        public uint? CurrentMinBidCentsAmount { get; set; }
        public List<SearchedBid> TripBids { get; set; }
    }
}
