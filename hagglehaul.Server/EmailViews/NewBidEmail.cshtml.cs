using Microsoft.AspNetCore.Mvc.RazorPages;


namespace hagglehaul.Server.EmailViews
{
    public class NewBidEmail
    {
        public string RiderName { get; set; }
        public string RiderEmail { get; set; }
        
        public double DriverRating { get; set; }
        
        public string DriverName { get; set; }
        public string DriverEmail { get; set; }
        public string DriverPhone { get; set; }

    }
}
