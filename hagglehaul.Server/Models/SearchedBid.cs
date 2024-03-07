namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying a bid in the search trips page.
    /// </summary>
    public class SearchedBid
    {
        /// <summary>
        /// The name of the driver.
        /// </summary>
        public string DriverName { get; set; }
        
        /// <summary>
        /// The rating of the driver, ranging from 1.0 to 5.0.
        /// </summary>
        public double? DriverRating { get; set; }
        
        /// <summary>
        /// The number of ratings the driver has received.
        /// </summary>
        public uint? DriverNumRatings { get; set; }
        
        /// <summary>
        /// The amount of the bid in cents.
        /// </summary>
        public uint CentsAmount { get; set; }
    }
}
