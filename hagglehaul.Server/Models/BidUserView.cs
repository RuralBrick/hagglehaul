namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying a bid in a trip.
    /// </summary>
    public class BidUserView
    {
        /// <summary>
        /// The ID of the bid.
        /// </summary>
        public string? BidId { get; set; }
        
        /// <summary>
        /// Whether the bid is the requesting user's bid, only applicable if the user is a driver.
        /// </summary>
        public bool YourBid { get; set; }
        
        /// <summary>
        /// The name of the driver.
        /// </summary>
        public string DriverName { get; set; } = null!;
        
        /// <summary>
        /// The rating of the driver, from 1.0 to 5.0.
        /// </summary>
        public double? DriverRating { get; set; }
        
        /// <summary>
        /// The number of ratings the driver has received.
        /// </summary>
        public uint? DriverNumRating { get; set; }
        
        /// <summary>
        /// The cost, in cents, of the bid.
        /// </summary>
        public uint Cost { get; set; }
    }
}
