namespace hagglehaul.Server.Models;

/// <summary>
/// Form data for giving a rating to a trip.
/// </summary>
public class GiveRating
{
    /// <summary>
    /// The ID of the trip to give a rating to. The party being rated is inferred from this.
    /// </summary>
    public string TripId { get; set; }
    
    /// <summary>
    /// The rating given to the party, a number from 1 to 5.
    /// </summary>
    public uint RatingGiven { get; set; }
}
