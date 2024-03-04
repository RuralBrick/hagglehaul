using hagglehaul.Server.Models;
using MongoDB.Driver;

namespace hagglehaul.Server.Services
{
    /// <summary>
    /// Corresponds to DriverProfile collection in the database.
    /// </summary>
    public interface IDriverProfileService
    {
        /// <summary>
        /// Gets a driver profile by the user's email
        /// </summary>
        /// <param name="email">The valid email address of an existing driver</param>
        /// <returns></returns>
        Task<DriverProfile> GetAsync(string email);
        /// <summary>
        /// Creates a new driver profile
        /// </summary>
        /// <param name="driverProfile">The <see cref="DriverProfile"/> object. Note that
        /// we do not check for correctness of the object.</param>
        /// <returns></returns>
        Task<DriverProfile> CreateAsync(DriverProfile driverProfile);
        /// <summary>
        /// Updates a driver profile
        /// </summary>
        /// <param name="email">The valid email address of an existing driver</param>
        /// <param name="driverProfileIn">The <see cref="DriverProfile"/> object. Note that
        /// we do not check for the correctness of the object.</param>
        /// <returns></returns>
        Task UpdateAsync(string email, DriverProfile driverProfileIn);
    }

    /// <summary>
    /// See <see cref="IDriverProfileService"/>.
    /// </summary>
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
        public async Task UpdateAsync(string email, DriverProfile driverProfileIn) =>
            await _driverProfileCollection.ReplaceOneAsync(driverProfile => driverProfile.Email == email, driverProfileIn);

        public async Task<DriverProfile> CreateAsync(DriverProfile driverProfile)
        {
            await _driverProfileCollection.InsertOneAsync(driverProfile);
            return driverProfile;
        }
    }
}
