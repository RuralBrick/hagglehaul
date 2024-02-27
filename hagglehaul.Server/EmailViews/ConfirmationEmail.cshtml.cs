using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hagglehaul.Server.EmailViews
{
    public class ConfirmationEmail
    {
        public string RiderName { get; set; }
        public string StartingLocation { get; set; }
        public string EndingLocation { get; set; }
        public decimal Price { get; set; }
        public string DriverName { get; set; }
    }
}
