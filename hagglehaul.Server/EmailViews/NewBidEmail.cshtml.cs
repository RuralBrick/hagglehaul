using System.ComponentModel.DataAnnotations;

namespace hagglehaul.Server.EmailViews
{
    /// <summary>
    /// The view for an email that notifies the rider when a new bid is placed.
    /// </summary>
    public class NewBidEmail
    {
        /// <summary>
        /// The name of the rider (to which the email is sent).
        /// </summary>
        public string RiderName { get; set; }

        /// <summary>
        /// The name of the trip.
        /// </summary>
        public string TripName { get; set; }
        
        /// <summary>
        /// The name of the driver who placed the bid.
        /// </summary>
        public string DriverName { get; set; }
        
        /// <summary>
        /// The rating of the driver, ranging from 1.0 to 5.0.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:0.##} ⭐", NullDisplayText = "unrated")]
        public double? DriverRating { get; set; }
        
        /// <summary>
        /// The cost of the trip, in dollars.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }

        /// <summary>
        /// The start time of the trip.
        /// </summary>
        public DateTime StartTime { get; set; }
        
        /// <summary>
        /// The pickup address of the trip.
        /// </summary>
        public string PickupAddress { get; set; }
        
        /// <summary>
        /// The destination address of the trip.
        /// </summary>
        public string DestinationAddress { get; set; }

        /// <summary>
        /// The email address of the rider (to which the email is sent).
        /// </summary>
        public string RiderEmail { get; set; }
    }
}
