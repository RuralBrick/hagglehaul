namespace hagglehaul.Server.EmailViews
{
    public class NewBidEmail
    {
        public string RiderName { get; set; }

        public string TripName { get; set; }
        public string DriverName { get; set; }
        public double DriverRating { get; set; }
        public decimal Price { get; set; } // In dollars

        public DateTime StartTime { get; set; }
        public string PickupAddress { get; set; }
        public string DestinationAddress { get; set; }

        public string RiderEmail { get; set; }
    }
}
