namespace hagglehaul.Server.Models
{
    public class RiderDashboard
    {
        public List<ConfirmedRiderTrip> ConfirmedTrips { get; set; } = null!;
        public List<UnconfirmedRiderTrip> TripsInBidding { get; set; } = null!;
        public List<ArchivedRiderTrip> ArchivedTrips { get; set; } = null!;
    }
}
