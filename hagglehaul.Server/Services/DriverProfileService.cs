using hagglehaul.Server.Models;

namespace hagglehaul.Server.Services
{
    public interface IDriverProfileService
    {
        Task<DriverProfile> GetAsync(string email);
    }

    public class DriverProfileService
    {
    }
}
