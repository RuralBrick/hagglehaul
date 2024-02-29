namespace hagglehaul.Server.Models
{
    public class DriverDashboard
    {
        public List<ConfirmedDriverTrip> ConfirmedTrips { get; set; } = null!;
        public List<UnconfirmedDriverTrip> TripsInBidding { get; set; } = null!;
        public List<ArchivedDriverTrip> ArchivedTrips { get; set; } = null!;
    }
}
