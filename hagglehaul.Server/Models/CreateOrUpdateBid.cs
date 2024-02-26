using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

[SwaggerSchema("Form to create or update a bid")]
public class CreateOrUpdateBid
{
    [SwaggerSchema("The trip ID to bid on")]
    public string TripId { get; set; }
    
    [SwaggerSchema("The amount of the bid in cents")]
    public uint CentsAmount { get; set; }
}
