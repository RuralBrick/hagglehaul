using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hagglehaul.Server.EmailViews
{
    public class AcceptedBidEmail
    {
        public string DriverName { get; set; }

        public string TripName { get; set; }
        public decimal Price { get; set; } // In dollars
        public string RiderName { get; set; }
        public string RiderPhone { get; set; }
        public string RiderEmail { get; set; }

        public DateTime StartTime { get; set; }
        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }

        public string DriverEmail { get; set; }
    }
}
