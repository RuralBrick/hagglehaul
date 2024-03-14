using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace hagglehaul.Server.EmailViews
{
    /// <summary>
    /// View for an email that notifies the driver when their bid is accepted.
    /// </summary>
    public class AcceptedBidEmail
    {
        /// <summary>
        /// Name of the driver.
        /// </summary>
        public string DriverName { get; set; }

        /// <summary>
        /// Name of the trip.
        /// </summary>
        public string TripName { get; set; }
        
        /// <summary>
        /// The cost of the trip, in dollars.
        /// </summary>
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        
        /// <summary>
        /// The name of the rider.
        /// </summary>
        public string RiderName { get; set; }
        
        /// <summary>
        /// The phone number of the rider.
        /// </summary>
        public string RiderPhone { get; set; }
        
        /// <summary>
        /// The email address of the rider.
        /// </summary>
        public string RiderEmail { get; set; }

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
        /// The email address of the driver (to which the email is sent).
        /// </summary>
        public string DriverEmail { get; set; }
    }
}
