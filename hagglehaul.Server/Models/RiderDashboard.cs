namespace hagglehaul.Server.Models
{
    public class RiderDashboard
    {
        public List<ConfirmedRiderTrip> ConfirmedTrips { get; set; } = null!;
        public List<UncomfirmedRiderTrip> TripsInBidding { get; set; } = null!;
        public List<ArchivedRiderTrip> ArchivedTrips { get; set; } = null!;
    }
}
