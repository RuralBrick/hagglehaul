namespace hagglehaul.Server.Models
{
    /// <summary>
    /// Data for displaying a driver's dashboard.
    /// </summary>
    public class DriverDashboard
    {
        /// <summary>
        /// The confirmed trips the driver has, i.e., the trips the driver has been selected for.
        /// </summary>
        public List<ConfirmedDriverTrip> ConfirmedTrips { get; set; } = null!;
        
        /// <summary>
        /// The trips in bidding, i.e., the trips the driver has bid on, but not yet been selected for.
        /// </summary>
        public List<UnconfirmedDriverTrip> TripsInBidding { get; set; } = null!;
        
        /// <summary>
        /// The archived trips the driver has, i.e., any trip of any state in the past.
        /// </summary>
        public List<ArchivedDriverTrip> ArchivedTrips { get; set; } = null!;
    }
}
