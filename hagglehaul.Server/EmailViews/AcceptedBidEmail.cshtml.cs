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
        public string PickupLocation { get; set; }
        public string DestinationLocation { get; set; }
        public decimal Price { get; set; }
        
        public decimal PickupTime { get; set; }

    }
}
