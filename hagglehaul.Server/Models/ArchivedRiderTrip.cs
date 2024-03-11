namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying a rider's archived trip.
    /// </summary>
    public class ArchivedRiderTrip
    {
        /// <summary>
        /// The Trip ID of the trip.
        /// </summary>
        public string TripId { get; set; } = null!;
        
        /// <summary>
        /// The name of the trip.
        /// </summary>
        public string TripName { get; set; } = null!;
        
        /// <summary>
        /// The thumbnail of the trip, PNG byte array.
        /// </summary>
        public byte[] Thumbnail { get; set; } = null!;
        
        /// <summary>
        /// The GeoJSON representation of the trip route.
        /// </summary>
        public string GeoJson { get; set; } = null!;
        
        /// <summary>
        /// The start time of the trip.
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// The distance, in meters, of the trip.
        /// </summary>
        public double? Distance { get; set; }
        
        /// <summary>
        /// The estimated duration, in seconds, of the trip.
        /// </summary>
        public double? Duration { get; set; }
        
        /// <summary>
        /// Whether the trip has a driver.
        /// </summary>
        public bool HasDriver { get; set; }
        
        /// <summary>
        /// The cost, in cents, of the trip.
        /// </summary>
        public uint Cost { get; set; }
        
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
        /// The pickup address of the trip.
        /// </summary>
        public string PickupAddress { get; set; } = null!;
        
        /// <summary>
        /// The destination address of the trip.
        /// </summary>
        public string DestinationAddress { get; set; } = null!;
    }
}
