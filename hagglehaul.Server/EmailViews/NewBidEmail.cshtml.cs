using Microsoft.AspNetCore.Mvc.RazorPages;


namespace hagglehaul.Server.EmailViews
{
    public class NewBidEmail
    {
        public string RiderName { get; set; }
        public string StartingLocation { get; set; }
        public string EndingLocation { get; set; }
    }
}
