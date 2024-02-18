namespace hagglehaul.Server.Models;

public class CreateOrUpdateBid
{
    public string TripId { get; set; }
    public uint CentsAmount { get; set; }
}
