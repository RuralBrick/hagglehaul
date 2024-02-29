using Microsoft.AspNetCore.Mvc.RazorPages;


namespace hagglehaul.Server.EmailViews
{
    public class NewBidEmail
    {
        public string RiderName { get; set; }
        public string RiderEmail { get; set; }
        public double Price { get; set; }
        public string PickupLocation { get; set; }
        public string StartTime { get; set; }
        public string DestinationLocation { get; set; }
        
        public double DriverRating { get; set; }
        
        public string DriverName { get; set; }
        public string DriverEmail { get; set; }
        public string DriverPhone { get; set; }

    }
}
