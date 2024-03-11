namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying the rider dashboard.
    /// </summary>
    public class RiderDashboard
    {
        /// <summary>
        /// The list of confirmed trips for the rider, i.e., trips that have a driver assigned.
        /// </summary>
        public List<ConfirmedRiderTrip> ConfirmedTrips { get; set; } = null!;
        
        /// <summary>
        /// The list of unconfirmed trips for the rider, i.e., trips that are still in the bidding process.
        /// </summary>
        public List<UnconfirmedRiderTrip> TripsInBidding { get; set; } = null!;
        
        /// <summary>
        /// The list of archived trips for the rider, i.e., any trip that is in the past.
        /// </summary>
        public List<ArchivedRiderTrip> ArchivedTrips { get; set; } = null!;
    }
}
