using Microsoft.AspNetCore.Mvc.RazorPages;

namespace hagglehaul.Server.EmailViews
{
    public class ConfirmationEmail
    {
        public string RiderName { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string DriverName { get; set; }
        public decimal Price { get; set; }
    }
}
