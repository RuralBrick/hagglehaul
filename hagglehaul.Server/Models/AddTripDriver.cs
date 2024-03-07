namespace hagglehaul.Server.Models;

/// <summary>
/// Form to select a bid for a trip.
/// </summary>
public class AddTripDriver
{
    /// <summary>
    /// The Trip ID of the trip to select a bid for.
    /// </summary>
    public string TripId { get; set; }
    
    /// <summary>
    /// The Bid ID of the bid to select.
    /// </summary>
    public string BidId { get; set; }
}