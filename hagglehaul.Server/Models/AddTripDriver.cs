using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

/// <summary>
/// Form to select a bid for a trip.
/// </summary>
[SwaggerSchema("Form to confirm a driver for a trip")]
public class AddTripDriver
{
    /// <summary>
    /// The Trip ID of the trip to select a bid for.
    /// </summary>
    [SwaggerSchema("The trip ID to confirm the driver on")]
    public string TripId { get; set; }
    
    /// <summary>
    /// The Bid ID of the bid to select.
    /// </summary>
    [SwaggerSchema("The bid ID to verify for the trip")]
    public string BidId { get; set; }
}