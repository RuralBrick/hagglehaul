using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    public interface IDriverProfileService
    {
        Task<DriverProfile> GetAsync(string email);
        Task<DriverProfile> CreateAsync(DriverProfile driverProfile);
    }

    public class DriverProfileService : IDriverProfileService
    {
        private readonly IMongoCollection<DriverProfile> _driverProfileCollection;

        public DriverProfileService(IMongoDatabase database)
        {
            _driverProfileCollection = database.GetCollection<DriverProfile>("DriverProfile");
        }

        public async Task<DriverProfile> GetAsync(string email)
        {
            var driverProfile = await _driverProfileCollection.Find(driverProfile => driverProfile.Email == email).FirstOrDefaultAsync();
            return driverProfile;
        }

        public async Task<DriverProfile> CreateAsync(DriverProfile driverProfile)
        {
            await _driverProfileCollection.InsertOneAsync(driverProfile);
            return driverProfile;
        }
    }
}
