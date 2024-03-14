using Swashbuckle.AspNetCore.Annotations;

namespace hagglehaul.Server.Models;

/// <summary>
/// Form data for giving a rating to a trip.
/// </summary>
[SwaggerSchema("Form to give a rating to a trip")]
public class GiveRating
{
    /// <summary>
    /// The ID of the trip to give a rating to. The party being rated is inferred from this.
    /// </summary>
    [SwaggerSchema("ID of the trip to rate a user on")]
    public string TripId { get; set; }
    
    /// <summary>
    /// The rating given to the party, a number from 1 to 5.
    /// </summary>
    [SwaggerSchema("Rating given from 1-5")]
    public uint RatingGiven { get; set; }
}
