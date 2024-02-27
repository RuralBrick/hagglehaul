using Microsoft.AspNetCore.Mvc.RazorPages;


namespace hagglehaul.Server.EmailViews
{
    public class NewBidEmail
    {
        public string RiderName { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
    }
}
