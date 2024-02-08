using hagglehaul.Server.Models;

namespace hagglehaul.Server.Services
{
    public interface IRiderProfileService
    {
        Task<RiderProfile> GetAsync(string email);
    }

    public class RiderProfileService
    {
    }
}
