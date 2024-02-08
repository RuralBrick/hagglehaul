namespace hagglehaul.Server.Models
{
    public interface IBidService
    {
        Task<List<Bid>> GetDriverBidsAsync(string email);
        Task<List<Bid>> GetTripBidsAsync(string tripId);
    }

    public class BidService
    {
    }
}
