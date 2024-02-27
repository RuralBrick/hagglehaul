using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hagglehaul.Server.EmailViews
{
    public class ConfirmationEmail
    {
        public string RiderName { get; set; }
        public double DriverRating { get; set; }
        public string DriverEmail { get; set; }
        public string DriverPhone { get; set; }
        public string PickupLocation { get; set; }
        public string DestinationLocation { get; set; }
        public decimal Price { get; set; }
        public string DriverName { get; set; }
    }
}
