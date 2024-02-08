using hagglehaul.Server.Models;

namespace hagglehaul.Server.Services
{
    public interface ITripService
    {
        Task<List<Trip>> GetAllTripsAsync();
        Task<List<Trip>> GetRiderTripsAsync(string email);
    }

    public class TripService
    {
    }
}
