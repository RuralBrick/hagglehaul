using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hagglehaul.Server.EmailViews
{
    public class AcceptedBidEmail
    {
        public string RiderName { get; set; }
        public string RiderEmail { get; set; }
        public string RiderPhone { get; set; }

        public string DriverName { get; set; }
        public string DriverEmail { get; set; }
        public string pickupLocation { get; set; }
        public string destinationLocation { get; set; }
        public decimal Price { get; set; }
        
        public decimal pickupTime { get; set; }

    }
}
