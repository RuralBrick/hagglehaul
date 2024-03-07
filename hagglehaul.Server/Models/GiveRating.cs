using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

[SwaggerSchema("Form to give a rating to a trip")]
public class GiveRating
{
    [SwaggerSchema("ID of the trip to rate a user on")]
    public string TripId { get; set; }

    [SwaggerSchema("Rating given from 1-5")]
    public uint RatingGiven { get; set; }
}
