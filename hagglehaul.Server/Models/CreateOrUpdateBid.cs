namespace hagglehaul.Server.Models;

/// <summary>
/// The form to create or update a bid.
/// </summary>
public class CreateOrUpdateBid
{
    /// <summary>
    /// The Trip ID of the trip to create or update a bid for.
    /// </summary>
    public string TripId { get; set; }
    
    /// <summary>
    /// The cost, in cents, of the bid.
    /// </summary>
    public uint CentsAmount { get; set; }
}
