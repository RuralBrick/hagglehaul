using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hagglehaul.Server.EmailViews
{
    public class AcceptedBidEmail
    {
        public string DriverName { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public decimal BidAmount { get; set; }
    }
}
