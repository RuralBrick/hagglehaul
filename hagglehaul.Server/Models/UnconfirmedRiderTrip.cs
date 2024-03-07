namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying an unconfirmed trip in the rider dashboard.
    /// </summary>
    public class UnconfirmedRiderTrip
    {
        /// <summary>
        /// The ID of the trip.
        /// </summary>
        public string TripID { get; set; } = null!;
        
        /// <summary>
        /// The name of the trip.
        /// </summary>
        public string TripName { get; set; } = null!;
        
        /// <summary>
        /// The thumbnail of the trip, as a base64-encoded PNG.
        /// </summary>
        public byte[] Thumbnail { get; set; } = null!;
        
        /// <summary>
        /// GeoJSON representing the trip route.
        /// </summary>
        public string GeoJson { get; set; } = null!;
        
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
        /// The list of bids placed on the trip.
        /// </summary>
        public List<BidUserView>? Bids { get; set; }
        
        /// <summary>
        /// The pickup address of the trip.
        /// </summary>
        public string PickupAddress { get; set; } = null!;
        
        /// <summary>
        /// The destination address of the trip.
        /// </summary>
        public string DestinationAddress { get; set; } = null!;
    }
}
