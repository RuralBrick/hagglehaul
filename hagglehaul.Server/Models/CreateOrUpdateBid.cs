using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

/// <summary>
/// The form to create or update a bid.
/// </summary>
[SwaggerSchema("Form to create or update a bid")]
public class CreateOrUpdateBid
{
    /// <summary>
    /// The Trip ID of the trip to create or update a bid for.
    /// </summary>
    [SwaggerSchema("The trip ID to bid on")]
    public string TripId { get; set; }
    
    /// <summary>
    /// The cost, in cents, of the bid.
    /// </summary>
    [SwaggerSchema("The amount of the bid in cents")]
    public uint CentsAmount { get; set; }
}
