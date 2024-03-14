namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for representing a trip in the trip search results.
    /// </summary>
    public class SearchedTrip
    {
        /// <summary>
        /// The ID of the trip.
        /// </summary>
        public string TripId { get; set; }
        
        /// <summary>
        /// The name of the trip.
        /// </summary>
        public string TripName { get; set; }
        
        /// <summary>
        /// The thumbnail of the trip, as a base64-encoded PNG.
        /// </summary>
        public byte[] Thumbnail { get; set; }
        
        /// <summary>
        /// The GeoJSON representing the trip route.
        /// </summary>
        public string GeoJson { get; set; }
        
        /// <summary>
        /// The start time of the trip.
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// The distance of the trip in meters.
        /// </summary>
        public double? Distance { get; set; }
        
        /// <summary>
        /// The estimated duration of the trip in seconds.
        /// </summary>
        public double? Duration { get; set; }
        
        /// <summary>
        /// The pickup address of the trip.
        /// </summary>
        public string PickupAddress { get; set; }
        
        /// <summary>
        /// The destination address of the trip.
        /// </summary>
        public string DestinationAddress { get; set; }
        
        /// <summary>
        /// The minimum bid amount in cents.
        /// </summary>
        public uint? CurrentMinBidCentsAmount { get; set; }
        
        /// <summary>
        /// The bids which have been placed on the trip.
        /// </summary>
        public List<SearchedBid> TripBids { get; set; }
    }
}
