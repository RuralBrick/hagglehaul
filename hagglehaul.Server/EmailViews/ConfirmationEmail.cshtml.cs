using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace hagglehaul.Server.EmailViews
{
    /// <summary>
    /// View for an email that notifies the rider with next steps after bid is accepted.
    /// </summary>
    public class ConfirmationEmail
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
        /// The name of the driver.
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
        /// The phone number of the driver.
        /// </summary>
        public string DriverPhone { get; set; }
        
        /// <summary>
        /// The email address of the driver.
        /// </summary>
        public string DriverEmail { get; set; }

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
        /// The email address of the rider (to which this email is sent).
        /// </summary>
        public string RiderEmail { get; set; }
    }
}
