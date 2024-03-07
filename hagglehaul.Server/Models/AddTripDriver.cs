using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

[SwaggerSchema("Form to confirm a driver for a trip")]
public class AddTripDriver
{
    [SwaggerSchema("The trip ID to confirm the driver on")]
    public string TripId { get; set; }

    [SwaggerSchema("The bid ID to verify for the trip")]
    public string BidId { get; set; }
}