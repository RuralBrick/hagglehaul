namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying a driver's confirmed trip.
    /// </summary>
    public class ConfirmedDriverTrip
    {
        /// <summary>
        /// The Trip ID of the trip.
        /// </summary>
        public string TripID { get; set; } = null!;
        
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
        /// The cost, in cents, of the trip.
        /// </summary>
        public uint Cost { get; set; }
        
        /// <summary>
        /// The name of the rider.
        /// </summary>
        public string RiderName { get; set; } = null!;
        
        /// <summary>
        /// The rating of the rider, from 1.0 to 5.0.
        /// </summary>
        public double? RiderRating { get; set; }
        
        /// <summary>
        /// The number of ratings the rider has received.
        /// </summary>
        public uint? RiderNumRating { get; set; }
        
        /// <summary>
        /// Whether the driver has rated the rider, and the trip is within the rating period.
        /// </summary>
        public bool ShowRatingPrompt { get; set; }
        
        /// <summary>
        /// The email address of the rider.
        /// </summary>
        public string RiderEmail { get; set; } = null!;
        
        /// <summary>
        /// The phone number of the rider.
        /// </summary>
        public string RiderPhone { get; set; } = null!;
        
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
